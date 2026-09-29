<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="UC0614_ExFinal.ChangePassword" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="cartao">
        <h1>Alterar palavra-passe</h1>

        <label for="txtPasswordAtual">Palavra-passe atual</label>
        <asp:TextBox ID="txtPasswordAtual" runat="server" TextMode="Password" />

        <label for="txtPasswordNova">Nova palavra-passe</label>
        <asp:TextBox ID="txtPasswordNova" runat="server" TextMode="Password" />

        <label for="txtConfirmarPassword">Confirmar nova palavra-passe</label>
        <asp:TextBox ID="txtConfirmarPassword" runat="server" TextMode="Password" />

        <asp:Button ID="btnAlterar"
                    runat="server"
                    Text="Alterar palavra-passe"
                    OnClick="btnAlterar_Click" />

        <asp:Label ID="lblMensagem"
                   runat="server"
                   CssClass="mensagem-erro" />
    </section>
</asp:Content>
