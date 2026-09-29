<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="UC0614_ExFinal.ForgotPassword" %>
<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <section class="cartao">

        <h1>Recuperar palavra-passe</h1>

        <p class="texto-ajuda">
            Introduza o email associado à sua conta.
        </p>

        <label for="txtEmail">Email</label>

        <asp:TextBox ID="txtEmail"
            runat="server"
            TextMode="Email" />

        <asp:Button ID="btnRecuperar"
            runat="server"
            Text="Enviar ligação de recuperação"
            OnClick="btnRecuperar_Click" />

        <asp:Label ID="lblMensagem"
            runat="server" />

    </section>

</asp:Content>