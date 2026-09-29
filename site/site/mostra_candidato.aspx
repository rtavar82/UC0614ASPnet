<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="mostra_candidato.aspx.cs" Inherits="site.mostra_candidato" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            esolhe o candidato:
            <asp:DropDownList ID="ddl_candidato" runat="server" DataSourceID="SqlDataSource" DataTextField="nome" DataValueField="num_cand" Width="436px">
            </asp:DropDownList>
            <asp:SqlDataSource ID="SqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:atec0226ConnectionString %>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT [num_cand], [nome] FROM [candidatos]" OnSelecting="SqlDataSource_Selecting"></asp:SqlDataSource>
        </div>
        <asp:Button ID="btn_ver" runat="server" OnClick="btn_ver_Click" Text="Ver" />
        <br />
        <br />
        Nome Candidato:<h3><asp:Label ID="lbl_nome" runat="server"></asp:Label> &nbsp;</h3>
        <br />
        Curso:<h3><asp:Label ID="lbl_curso" runat="server"></asp:Label>&nbsp;</h3>
        <br />
        <asp:Image ID="img_foto" runat="server" />
        <br />
        <br />
        <asp:Literal ID="lt_carta" runat="server"></asp:Literal>
    </form>
</body>
</html>
