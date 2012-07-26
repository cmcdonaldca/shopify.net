<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Authorize.aspx.cs" Inherits="SampleWebApplication.Authorize" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Panel ID="ShopNameForm" runat="server">
    <p>Please enter the url of your store (i.e. colinstore.myshopify.com)</p>
    <p>Store Name: <asp:TextBox ID="ShopName" runat="server"></asp:TextBox> .myshopify.com</p>
    <br />
    <asp:Button ID="GetAuthorization" runat="server" Text="Submit" OnClick="GetAuthorization_Click" />
    
    </asp:Panel>
    </div>
    </form>
</body>
</html>
