<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ler_imagem.aspx.cs" Inherits="site.ler_imagem" %>

<!DOCTYPE html>

<title></title></head><body>
    <form id="form1" runat="server">
        <div>
            Imagem:<asp:DropDownList ID="ddl_imagem" runat="server" DataSourceID="SqlDataSource" DataTextField="nome" DataValueField="id_ficheiro" Height="22px" Width="268px">
            </asp:DropDownList>
            <asp:SqlDataSource ID="SqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:atec0226ConnectionString %>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT * FROM [ficheiros]"></asp:SqlDataSource>&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_mostrar" runat="server" OnClick="btn_mostrar_Click" Text="Mostrar" />
        </div>
    </form></body></html>