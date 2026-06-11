const express = require("express");
const session = require("express-session");
const bcrypt = require("bcryptjs");
const nodemailer = require("nodemailer");
const mysql = require("mysql2");
const dotenv = require("dotenv");
dotenv.config();

const app = express();
const PORT = 3000;

// Middleware base
app.use(express.urlencoded({ extended: true }));
app.use(express.json());
app.use(express.static("public"));

// Sessões
app.use(session({
    secret: "segredo-super-forte",
    resave: false,
    saveUninitialized: true
}));

// MySQL
const db = mysql.createConnection({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASS,
    database: process.env.DB_NAME
});

db.connect(err => {
    if (err) throw err;
    console.log("MySQL ligado com sucesso");
});

// Email
const transporter = nodemailer.createTransport({
    service: "gmail",
    auth: {
        user: process.env.EMAIL,
        pass: process.env.EMAIL_PASS
    }
});

// =========================
//  MIDDLEWARE PARA PROTEGER ROTAS
// =========================
function protegerRota(req, res, next) {
    if (!req.session.user) {
        return res.redirect("/index.html");
    }
    next();
}

// =========================
//  ROTA DO DASHBOARD (PROTEGIDA)
// =========================
app.get("/dashboard", protegerRota, (req, res) => {
    res.sendFile(__dirname + "/public/dashboard.html");
});

// =========================
//  SERVIR verificar.html BONITO
// =========================
app.get("/verificar.html", (req, res) => {
    res.sendFile(__dirname + "/public/verificar.html");
});

// =========================
//  REGISTO — PASSO 1 (ENVIA CÓDIGO)
// =========================
app.post("/auth/registo", async (req, res) => {
    const { nome, email, senha } = req.body;

    db.query("SELECT * FROM users WHERE email = ?", [email], async (err, result) => {
        if (err) return res.status(500).json({ error: "Erro no servidor." });

        if (result.length > 0) {
            return res.status(400).json({ error: "Este e-mail já está registado." });
        }

        const codigo = Math.floor(100000 + Math.random() * 900000);

        req.session.tempUser = { nome, email, senha, codigo };

        try {
            await transporter.sendMail({
                from: `"Segurança do Site" <${process.env.EMAIL}>`,
                to: email,
                subject: "Código de Verificação de Registo",
                html: `
                    <div style="font-family: Arial; padding: 20px;">
                        <h2>🔐 Verificação de Registo</h2>
                        <p>Olá, <strong>${nome}</strong>!</p>
                        <p>O seu código de verificação é:</p>
                        <div style="
                            font-size: 32px;
                            font-weight: bold;
                            text-align: center;
                            margin: 20px 0;
                            padding: 15px;
                            background: #e8f0fe;
                            border-radius: 8px;
                            color: #1a73e8;
                            letter-spacing: 4px;
                        ">
                            ${codigo}
                        </div>
                        <p>Se não tentou criar conta, ignore este email.</p>
                    </div>
                `
            });
        } catch (err) {
            return res.status(500).json({ error: "Erro ao enviar email." });
        }

        return res.json({ ok: true, redirect: "/verificar.html?registo=1" });
    });
});

// =========================
//  REGISTO — PASSO 2 (VALIDA CÓDIGO)
// =========================
app.post("/auth/verificar-2fa", async (req, res) => {
    const { codigo } = req.body;

    if (!req.session.tempUser) {
        return res.status(400).send("Sessão expirada.");
    }

    if (codigo != req.session.tempUser.codigo) {
        return res.status(400).send("Código incorreto.");
    }

    const hashed = await bcrypt.hash(req.session.tempUser.senha, 10);

    db.query(
        "INSERT INTO users (nome, email, senha) VALUES (?, ?, ?)",
        [req.session.tempUser.nome, req.session.tempUser.email, hashed],
        (err) => {
            if (err) throw err;

            req.session.tempUser = null;

            res.redirect("/index.html");
        }
    );
});

// =========================
//  LOGIN — PASSO 1 (ENVIA CÓDIGO)
// =========================
app.post("/auth/login", (req, res) => {
    const { email, senha } = req.body;

    db.query("SELECT * FROM users WHERE email = ?", [email], async (err, result) => {
        if (err) throw err;

        if (result.length === 0) {
            return res.send("Credenciais inválidas.");
        }

        const user = result[0];
        const ok = await bcrypt.compare(senha, user.senha);

        if (!ok) {
            return res.send("Credenciais inválidas.");
        }

        const codigo = Math.floor(100000 + Math.random() * 900000);

        req.session.loginUser = {
            id: user.id,
            nome: user.nome,
            email: user.email,
            codigo
        };

        await transporter.sendMail({
            from: `"Segurança do Site" <${process.env.EMAIL}>`,
            to: user.email,
            subject: "Código de Verificação - Login",
            html: `
                <div style="font-family: Arial; padding: 20px;">
                    <h2>🔐 Verificação de Login</h2>
                    <p>Olá, <strong>${user.nome}</strong>!</p>
                    <p>O seu código de login é:</p>
                    <div style="
                        font-size: 32px;
                        font-weight: bold;
                        text-align: center;
                        margin: 20px 0;
                        padding: 15px;
                        background: #e8f0fe;
                        border-radius: 8px;
                        color: #1a73e8;
                        letter-spacing: 4px;
                    ">
                        ${codigo}
                    </div>
                    <p>Se não tentou iniciar sessão, ignore este email.</p>
                </div>
            `
        });

        res.redirect("/verificar.html?login=1");
    });
});

// =========================
//  LOGIN — PASSO 2 (VALIDA CÓDIGO)
// =========================
app.post("/auth/verificar-login", (req, res) => {
    const { codigo } = req.body;

    if (!req.session.loginUser) {
        return res.send("Sessão expirada.");
    }

    if (codigo != req.session.loginUser.codigo) {
        return res.send("Código incorreto.");
    }

    req.session.user = {
        id: req.session.loginUser.id,
        nome: req.session.loginUser.nome,
        email: req.session.loginUser.email
    };

    req.session.loginUser = null;

    res.redirect("/dashboard");
});

// =========================
//  LOGOUT
// =========================
app.get("/auth/logout", (req, res) => {
    req.session.destroy(() => {
        res.redirect("/index.html");
    });
});

// =========================
//  INICIAR SERVIDOR
// =========================
app.listen(PORT, () => {
    console.log(`Servidor a correr em http://localhost:${PORT}`);
});
