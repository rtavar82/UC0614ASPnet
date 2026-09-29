<%@ Page Title="Entrar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UC0614_ExFinal.Login" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="cartao">
        <h1>Iniciar sessão</h1><p class="texto-ajuda">Introduza as credenciais da sua conta.</p>
        <label for="txtUtilizador">Utilizador ou email</label><asp:TextBox ID="txtUtilizador" runat="server" />
        <label for="txtPassword">Palavra-passe</label><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
        <asp:Button ID="btnEntrar" runat="server" Text="Entrar" OnClick="btnEntrar_Click" />
        <div class="separador"><span>ou</span></div>
        <p class="texto-ajuda">
            <a runat="server" href="~/ForgotPassword.aspx">
                Esqueci-me da palavra-passe
            </a>
        </p>
        <asp:Button
            ID="btnGoogle"
            runat="server"
            Text="Continuar com Google"
            CssClass="botao-social"
            OnClick="btnGoogle_Click" />
        <asp:Button
            ID="btnFacebook"
            runat="server"
            Text="Continuar com Facebook"
            CssClass="botao-social botao-facebook"
            OnClick="btnFacebook_Click" />
        <asp:Label ID="lblMensagem" runat="server" CssClass="mensagem-erro" />
        <p class="texto-ajuda">Ainda não tem conta? <a href="Register.aspx">Registe-se aqui</a>.</p>
    </section>
</asp:Content>
