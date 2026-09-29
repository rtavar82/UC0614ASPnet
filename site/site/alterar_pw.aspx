<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="alterar_pw.aspx.cs" Inherits="site.alterar_pw" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Palavra-passe atual:<asp:TextBox ID="tb_pw_atual" runat="server"></asp:TextBox>
            <br />
            <br />
            Palavra-Pass Nova:<asp:TextBox ID="tb_pw_nova" runat="server"></asp:TextBox>
            <br />
        </div>
        <asp:Button ID="btn_alterar" runat="server" OnClick="btn_alterar_Click" Text="alterar" />
        <br />
        <br />
        <asp:Label ID="lbl_mensagem" runat="server"></asp:Label>
    </form>
</body>
</html>
