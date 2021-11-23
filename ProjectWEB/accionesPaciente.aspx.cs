﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using modelo;
using negocio;
using validarPermiso;

namespace ProjectWEB
{
    public partial class agregarPaciente : System.Web.UI.Page
    {
        public Paciente paciente;
        public static string pacienteDniEditar;
        public List<Permiso> permisosList;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["id"] != null)
            {
                validarAccion("Editar pacientes");

                LabelTitulo.Text = "Editar Paciente";
                ButtonAgregarModificar.Text = "Confirmar";

                paciente = new Paciente();
                string id = (string)Request.QueryString["id"];
                PacienteNegocio pacNego = new PacienteNegocio();
                paciente = pacNego.buscar("id", id);

                if (!IsPostBack)
                {
                    pacienteDniEditar = paciente.dni;

                    TextBoxApellido.Text = paciente.apellido;
                    TextBoxNombre.Text = paciente.nombre;
                    TextBoxDni.Text = paciente.dni;
                    TextBoxTelefono.Text = paciente.telefono;
                    TextBoxEmail.Text = paciente.email;
                }
            }
            else
            {
                validarAccion("Agregar pacientes");
            }
            if (Request.QueryString["noRegistrado"] != null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ranbomtext", "no_registrado()", true);
            }
        }

        protected void ButtonAgregarModificar_Click(object sender, EventArgs e)
        {
           bool validar = validar_paciente_existente();
            if (validar == true)
            {
                if (paciente == null) paciente = new Paciente();

                paciente.nombre = TextBoxNombre.Text;
                paciente.apellido = TextBoxApellido.Text;
                paciente.dni = TextBoxDni.Text;
                paciente.telefono = TextBoxTelefono.Text;
                paciente.email = TextBoxEmail.Text;


                string confirmacion = "";
                PacienteNegocio pacNego = new PacienteNegocio();
                if (paciente.id != 0)
                {
                    pacNego.modificar(paciente);
                    confirmacion = "modificado";
                }
                else
                {
                    pacNego.agregar(paciente);
                    confirmacion = "agregado";
                }
                Response.Redirect("inicio.aspx?accion=" + confirmacion);
            }
        }
        public bool validar_paciente_existente()
        {
            LabelError.Text = "";
            PacienteNegocio pacNego = new PacienteNegocio();
            paciente = pacNego.buscar("dni", TextBoxDni.Text);
            if (paciente.id != 0)
            {
                if(pacienteDniEditar != null && pacienteDniEditar == TextBoxDni.Text)
                {
                    return true;
                }
                LabelError.Text = "*El paciente ya se encuentra registrado";
                return false;
            }
            EmpleadoNegocio empNego = new EmpleadoNegocio();
            Empleado emp = new Empleado();
            emp = empNego.buscar(TextBoxDni.Text);
            if (emp.idEmpleado != 0)
            {
                LabelError.Text = "*Error el DNI ingresado pertenece a un empleado";
                return false;
            }
            return true;
        }
        public void validarAccion(string val)
        {
            permisosList = new List<Permiso>();
            permisosList = (List<Permiso>)Session["permisos"];
            ValidarPermiso valPer = new ValidarPermiso();
            if(permisosList == null || valPer.validar_permiso(permisosList,val) == false)
            {
                Response.Redirect("inicio.aspx");
            }
        }
    }
}