<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="inserir_ficheiro.aspx.cs" Inherits="site.inserir_ficheiro" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Nome do ficheiro:
            <asp:TextBox ID="tb_ficheiro" runat="server"></asp:TextBox>
            <br />
            <br />
            Escolher ficheiro:
            <asp:FileUpload ID="FileUpload1" runat="server" Width="645px" />
        </div>
        <asp:Button ID="btn_adicionar" runat="server" OnClick="btn_adicionar_Click" Text="adicionar" />
    </form>
</body>
</html>
