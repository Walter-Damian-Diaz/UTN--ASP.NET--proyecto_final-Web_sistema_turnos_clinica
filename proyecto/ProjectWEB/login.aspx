﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="ProjectWEB.log" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>

    <link href="~/Content/estilos/global.css" rel="stylesheet" />
    <link href="~/Content/bootstrap5/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.3.1/css/all.css">
    <link href="https://fonts.googleapis.com/css2?family=Abril+Fatface&display=swap" rel="stylesheet">

    <style>
        body{
            background: linear-gradient(90deg, rgba(2,0,36,1) 0%, rgba(9,121,55,1) 0%, rgba(0,212,255,1) 100%);
        }
        li{
            list-style:none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div class="container text-center text-light mt-5">
            <h1 class="text-center mt-3" style="font-family: 'Abril Fatface', cursive;">Iniciar Sesion</h1>
            <i class="fas fa-users" style="font-size:5rem;"></i>
        </div>

        <div class="container mt-3">
            <div class="row">

                <div class="col-2 col-md-3">

                </div>

                <div class="col-8 col-md-6 shadow p-3">
                    <li>
                        <label class="form-label">Usuario: </label>
                        <asp:TextBox class="form-control" ID="TextBoxUsuario" runat="server" ClientIDMode="Static"></asp:TextBox>
                    </li>
                    <li>
                        <label class="form-label">Contaseña: </label>
                        <asp:TextBox ID="TextBoxClave" runat="server" class="form-control" TextMode="Password" ClientIDMode="Static" type="password"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Label ID="LabelIncorrecto" runat="server" Text="" ForeColor="Red" ClientIDMode="Static"></asp:Label>
                    </li>
                    <asp:Button class="btn btn-success mt-3" ID="Button1" runat="server" Text="Ingresar" OnClientClick="return validar_ingreso_usuario()" OnClick="Button1_Click"/>
                </div>
                
                <div class="col-2 col-md-3">

                </div>
              

            </div>
        </div>
    </form>
</body>
</html>
