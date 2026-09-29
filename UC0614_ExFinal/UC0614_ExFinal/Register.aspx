<%@ Page Title="Registo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="UC0614_ExFinal.Register" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="cartao">
        <h1>Criar conta</h1><p class="texto-ajuda">A conta ficará pendente até à confirmação por email.</p>
        <label for="txtNome">Nome</label><asp:TextBox ID="txtNome" runat="server" />
        <label for="txtEmail">Email</label><asp:TextBox ID="txtEmail" runat="server" TextMode="Email" />
        <label for="txtPassword">Palavra-passe</label><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
        <label for="txtConfirmarPassword">Confirmar palavra-passe</label><asp:TextBox ID="txtConfirmarPassword" runat="server" TextMode="Password" />
        <asp:Button ID="btnRegistar" runat="server" Text="Registar" OnClick="btnRegistar_Click" />
        <asp:Label ID="lblMensagem" runat="server" CssClass="mensagem-erro" />
    </section>
</asp:Content>
