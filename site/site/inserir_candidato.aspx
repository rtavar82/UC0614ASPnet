<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="inserir_candidato.aspx.cs" Inherits="site.inserir_candidato" ValidateRequest="false" %><!DOCTYPE html><html xmlns="http://www.w3.org/1999/xhtml"><head runat="server">
    <title></title></head><body>
    <form id="form1" runat="server">
        <div>
            nome:
                <asp:TextBox ID="tb_nome" runat="server"></asp:TextBox>
            <p>
                curso:
                <asp:DropDownList ID="ddl_curso" runat="server">
                    <asp:ListItem>CET Robotica</asp:ListItem>
                    <asp:ListItem>CET Informatica</asp:ListItem>
                    <asp:ListItem>CET Comunicação</asp:ListItem>
                    <asp:ListItem>CET Redes</asp:ListItem>
                    <asp:ListItem>CET TPSI</asp:ListItem>
                </asp:DropDownList>
            </p>
            <p>
                Foto:
                <asp:FileUpload ID="fu_foto" runat="server" Width="618px" />
            </p>
            <p>
                carta de apresentação:
            </p>
            <script src="ckeditor/ckeditor.js"></script>
            <asp:TextBox ID="tb_carta" runat="server" TextMode="MultiLine" Width="743px"></asp:TextBox>
            <script>
                CKEDITOR.replace('<%=tb_carta.ClientID%>', {
                    customConfig: 'custom/editor_config.js'
                });
            </script>
            <br />
            <p>
                <asp:Button ID="btn_inserir" runat="server" OnClick="btn_inserir_Click" Text="inserir" />
            </p>
        </div>
    </form>
</body>
</html>