<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="UC0614_ExFinal.ResetPassword" %>
<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <section class="cartao">

        <h1>Definir nova palavra-passe</h1>

        <label for="txtPasswordNova">Nova palavra-passe</label>

        <asp:TextBox ID="txtPasswordNova"
            runat="server"
            TextMode="Password" />

        <label for="txtConfirmarPassword">
            Confirmar nova palavra-passe
        </label>

        <asp:TextBox ID="txtConfirmarPassword"
            runat="server"
            TextMode="Password" />

        <asp:Button ID="btnRepor"
            runat="server"
            Text="Alterar palavra-passe"
            OnClick="btnRepor_Click" />

        <asp:Label ID="lblMensagem"
            runat="server" />

    </section>

</asp:Content>