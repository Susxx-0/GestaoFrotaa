const express = require('express');
const bcrypt = require('bcryptjs');
const session = require('express-session');
const nodemailer = require('nodemailer');
require('dotenv').config();

const app = express();

// Configurações do Express
app.use(express.urlencoded({ extended: true }));
app.use(express.static('public'));
app.use(session({
    secret: 'meusegredomuitoseguro123',
    resave: false,
    saveUninitialized: true
}));

// Base de dados simulada em memória
const utilizadoresDB = [];

// Configuração do Nodemailer (Lê os dados do teu ficheiro .env)
const transportador = nodemailer.createTransport({
    host: process.env.EMAIL_HOST,
    port: process.env.EMAIL_PORT,
    secure: process.env.EMAIL_PORT == 465,
    auth: {
        user: process.env.EMAIL_USER,
        pass: process.env.EMAIL_PASS
    }
});

// ROTA 1: Registo de Utilizador
app.post('/auth/registo', async (req, res) => {
    const { nome, email, senha } = req.body;
    const senhaEncriptada = await bcrypt.hash(senha, 10);

    utilizadoresDB.push({
        nome,
        email,
        senha: senhaEncriptada,
        codigo2FA: null
    });

    console.log("Utilizador registado!", utilizadoresDB);
    res.send('<h2>Conta criada com sucesso! <a href="/index.html">Ir para o Início de Sessão</a></h2>');
});

// ROTA 2: Primeiro Passo do Início de Sessão (Email e Palavra-passe)
app.post('/auth/login', async (req, res) => {
    const { email, senha } = req.body;
    const utilizador = utilizadoresDB.find(u => u.email === email);

    if (!utilizador) {
        return res.send('Utilizador não encontrado. <a href="/index.html">Tentar novamente</a>');
    }

    const senhaCorreta = await bcrypt.compare(senha, utilizador.senha);
    if (!senhaCorreta) {
        return res.send('Palavra-passe incorreta. <a href="/index.html">Tentar novamente</a>');
    }

    // GERAR CÓDIGO 2FA (6 dígitos)
    const codigoGerado = Math.floor(100000 + Math.random() * 900000).toString();
    utilizador.codigo2FA = codigoGerado;
    req.session.emailEmTentativa = email;

    // CONFIGURAR O E-MAIL
    const opcoesEmail = {
        from: `"Segurança do Site" <${process.env.EMAIL_USER}>`,
        to: utilizador.email,
        subject: 'O seu código de verificação de 2 fatores',
        html: `
            <div style="font-family: Arial, sans-serif; padding: 20px; border: 1px solid #eee; border-radius: 8px; max-width: 500px;">
                <h2>Olá, ${utilizador.nome}!</h2>
                <p>Para concluir o seu início de sessão, introduza o seguinte código de segurança:</p>
                <div style="font-size: 24px; font-weight: bold; color: #007bff; background: #f4f4f4; padding: 10px; text-align: center; letter-spacing: 5px; border-radius: 4px;">
                    ${codigoGerado}
                </div>
                <p style="color: #777; font-size: 12px; margin-top: 20px;">Se não tentou iniciar sessão, por favor ignore este e-mail.</p>
            </div>
        `
    };

    // ENVIAR O E-MAIL
    try {
        await transportador.sendMail(opcoesEmail);
        console.log(`E-mail enviado para ${utilizador.email} com o código: ${codigoGerado}`);
        res.redirect('/verificar.html');
    } catch (error) {
        console.error("Erro ao enviar e-mail:", error);
        res.send('Erro ao enviar o e-mail de verificação. Verifique as configurações do servidor.');
    }
});

// ROTA 3: Segundo Passo (Validar o Código)
app.post('/auth/verificar-2fa', (req, res) => {
    const { codigo } = req.body;
    const email = req.session.emailEmTentativa;

    if (!email) {
        return res.send('Sessão expirada. <a href="/index.html">Inicie sessão novamente</a>');
    }

    const utilizador = utilizadoresDB.find(u => u.email === email);

    if (utilizador && utilizador.codigo2FA === codigo) {
        utilizador.codigo2FA = null;
        req.session.utilizadorLogado = utilizador;
        res.send(`<h1>🎉 Bem-vindo, ${utilizador.nome}! Entrou com sucesso usando o 2FA por E-mail!</h1>`);
    } else {
        res.send('Código inválido ou incorreto! <a href="/verificar.html">Tentar novamente</a>');
    }
});

// A mensagem com o foguetão que vai aparecer no teu terminal!
app.listen(3000, () => {
    console.log('🚀 Servidor a correr em http://localhost:3000');
});