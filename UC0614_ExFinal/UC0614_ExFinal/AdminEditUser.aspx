<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="AdminEditUser.aspx.cs" Inherits="UC0614_ExFinal.AdminEditUser" %>
<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <section class="cartao">

        <h1>Editar utilizador</h1>

        <label for="txtNome">Nome</label>
        <asp:TextBox ID="txtNome"
            runat="server" />

        <label for="txtEmail">Email</label>
        <asp:TextBox ID="txtEmail"
            runat="server"
            TextMode="Email" />

        <label for="ddlPerfil">Perfil</label>
        <asp:DropDownList ID="ddlPerfil"
            runat="server">

            <asp:ListItem
                Text="Administrador"
                Value="1" />

            <asp:ListItem
                Text="Utilizador"
                Value="2" />

        </asp:DropDownList>

        <label for="ddlEstado">Estado</label>
        <asp:DropDownList ID="ddlEstado"
            runat="server">

            <asp:ListItem
                Text="Ativo"
                Value="1" />

            <asp:ListItem
                Text="Pendente"
                Value="0" />

        </asp:DropDownList>

        <asp:Button ID="btnGuardar"
            runat="server"
            Text="Guardar alterações"
            OnClick="btnGuardar_Click" />

        <a class="botao"
            href="AdminUsers.aspx">
            Cancelar
        </a>

        <br />

        <asp:Label ID="lblMensagem"
            runat="server" />

    </section>

</asp:Content>
