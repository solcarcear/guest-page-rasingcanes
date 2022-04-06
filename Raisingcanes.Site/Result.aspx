<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Result.aspx.cs" Inherits="Raisingcanes.Site.Result" %>

<!DOCTYPE html>
<html lang="en">

<head>

    <meta charset="utf-8">
    <title>Raising Cane's</title>
    <meta name="description" content="">
    <meta name="author" content="Praxi">

    <!-- Favicons-->
    <link rel="shortcut icon" href="theme/img/logo-mobile.jpg" type="image/x-icon" />
    <link rel="apple-touch-icon" type="image/x-icon" href="theme/img/Icon57x57.jpg" />
    <link rel="apple-touch-icon" type="image/x-icon" sizes="72x72" href="theme/img/Icon72x72.jpg" />
    <link rel="apple-touch-icon" type="image/x-icon" sizes="114x114" href="theme/img/Icon114x114.jpg" />
    <link rel="apple-touch-icon" type="image/x-icon" sizes="144x144" href="theme/img/Icon144x144.jpg" />

    <!-- Mobile Specific Metas -->
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <!-- CSS -->
    <link href="theme/css/style.css" rel="stylesheet">
    
   
</head>
<body">
    <div id="complete" runat="server" visible="false" class="container">
        <table style="width: 100%">
            <tr>
                <td></td>
                <td>
                    <asp:Image ID="Image2" runat="server" ImageUrl="theme/img/logo.png" Style="float: right; width: 30%; height: auto;" /></td>
            </tr>
            <tr>
                <td colspan="2">
                    <h3 class="h3Title"><%=Resources.Messages.MSGOK%></h3>
                </td>
            </tr>
        </table>
    </div>
    <div id="error" runat="server" visible="false" class="container">
        <table style="width: 100%">
            <tr>
                <td></td>
                <td>
                    <asp:Image ID="Image1" runat="server" ImageUrl="theme/img/logo.png" Style="float: right; width: 30%; height: auto;" /></td>
            </tr>
            <tr>
                <td colspan="2">
                    <h3 class="h3Title"><%=Resources.Messages.MSGNOOK%></h3>
                </td>
            </tr>
        </table>
    </div>
</body>
</html>
