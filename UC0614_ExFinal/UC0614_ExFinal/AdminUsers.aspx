<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="AdminUsers.aspx.cs" Inherits="UC0614_ExFinal.AdminUsers" %>
<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <section class="cartao largo">

        <h1>Gestão de utilizadores</h1>

        <p class="texto-ajuda">
            Consulte, insira, altere ou elimine utilizadores da aplicação.
        </p>

        <a class="botao" href="AdminCreateUser.aspx">
            Inserir utilizador
        </a>

        <asp:GridView ID="gvUtilizadores"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="tabela"
            GridLines="None"
            OnRowCommand="gvUtilizadores_RowCommand">

            <Columns>

                <asp:BoundField
                    DataField="Nome"
                    HeaderText="Utilizador" />

                <asp:BoundField
                    DataField="Email"
                    HeaderText="Email" />

                <asp:BoundField
                    DataField="Perfil"
                    HeaderText="Perfil" />

                <asp:BoundField
                    DataField="Estado"
                    HeaderText="Estado" />

                <asp:TemplateField HeaderText="Ações">
                    <ItemTemplate>

                        <a href='<%# "AdminEditUser.aspx?id=" + Eval("Id") %>'>
                            Editar
                        </a>

                        &nbsp; | &nbsp;

                        <asp:LinkButton ID="btnEliminar"
                            runat="server"
                            Text="Eliminar"
                            CommandName="Eliminar"
                            CommandArgument='<%# Eval("Id") %>'
                            OnClientClick="return confirm('Tem a certeza que pretende eliminar este utilizador?');" />

                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>

            <EmptyDataTemplate>
                Não existem utilizadores registados.
            </EmptyDataTemplate>

        </asp:GridView>

        <asp:Label ID="lblMensagem"
            runat="server" />

    </section>

</asp:Content>