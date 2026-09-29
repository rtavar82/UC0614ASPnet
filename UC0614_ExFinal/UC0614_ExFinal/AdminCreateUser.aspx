<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="AdminCreateUser.aspx.cs" Inherits="UC0614_ExFinal.AdminCreateUser" %>
<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <section class="cartao">

        <h1>Inserir utilizador</h1>

        <label for="txtNome">Nome</label>
        <asp:TextBox ID="txtNome"
            runat="server" />

        <label for="txtEmail">Email</label>
        <asp:TextBox ID="txtEmail"
            runat="server"
            TextMode="Email" />

        <label for="txtPassword">Palavra-passe</label>
        <asp:TextBox ID="txtPassword"
            runat="server"
            TextMode="Password" />

        <label for="txtConfirmarPassword">
            Confirmar palavra-passe
        </label>
        <asp:TextBox ID="txtConfirmarPassword"
            runat="server"
            TextMode="Password" />

        <label for="ddlPerfil">Perfil</label>
        <asp:DropDownList ID="ddlPerfil"
            runat="server">

            <asp:ListItem
                Text="Utilizador"
                Value="2" />

            <asp:ListItem
                Text="Administrador"
                Value="1" />

        </asp:DropDownList>

        <label for="ddlEstado">Estado</label>
        <asp:DropDownList ID="ddlEstado"
            runat="server">

            <asp:ListItem
                Text="Ativo"
                Value="1" />

            <asp:ListItem
                Text="Pendente - ativação por email"
                Value="0" />

        </asp:DropDownList>

        <asp:Button ID="btnCriar"
            runat="server"
            Text="Criar utilizador"
            OnClick="btnCriar_Click" />

        <a class="botao"
           href="AdminUsers.aspx">
            Cancelar
        </a>

        <br />

        <asp:Label ID="lblMensagem"
            runat="server" />

    </section>

</asp:Content>