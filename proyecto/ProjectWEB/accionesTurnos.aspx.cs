﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using modelo;
using negocio;
using validarPermiso;
using emailServer;

namespace ProjectWEB
{
    public partial class verTurnos : System.Web.UI.Page
    {
        public static Turno turno;
        public Paciente paciente;
        public static int horaEditando=99;
        public List<Doctor> doctoresList;
        public List<Doctor> doctoresList2;
        public List<Paciente> pacientesList;
        public List<Turno> turnosList;
        public List<Horario> horariosList;
        public List<Permiso> permisosList;
        public string noRegistrado ;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                EspecialidadNegocio espNego = new EspecialidadNegocio();
                DropEspecialidad.DataSource = espNego.listar_especilidades_para_turnos();
                DropEspecialidad.DataValueField = "id";
                DropEspecialidad.DataTextField = "nombre";
                DropEspecialidad.DataBind();
                DropEspecialidad.Items.Insert(0, new ListItem("[Seleccionar]", "0"));

                if (Request.QueryString["id"] != null)
                {
                    validar_permisos("Editar turnos");

                    LabelTitulo.Text = "Editar Turno";
                    ButtonReservar.Text = "Confirmar";
                    turno = new Turno();
                    int id = Convert.ToInt32(Request.QueryString["id"]);
                    TurnoNegocio turNego = new TurnoNegocio();
                    turno = turNego.buscar_por_id(id);

                    TextBoxDni.Text = turno.paciente.dni;
                    TextBoxfecha.Text = String.Format("{0:yyyy-MM-dd}", turno.fecha);
                    DropEspecialidad.SelectedIndex = turno.especialidad.id;

                    horaEditando = turno.hora;
                    int[] horariosDisponibles = horarios_disponibles();
                    DropHora.DataSource = horariosDisponibles;
                    DropHora.DataBind();
                    DropHora.SelectedValue=horaEditando.ToString();
                    horaEditando = 99;

                    DoctorNegocio docNego = new DoctorNegocio();
                    doctoresList = new List<Doctor>();
                    doctoresList = docNego.listar("especialidades con turno disponible", turno.especialidad.id);
                    DropPersonalDisponible.DataSource = doctoresList;
                    DropPersonalDisponible.DataValueField = "id";
                    DropPersonalDisponible.DataTextField = "nombreCompleto";
                    DropPersonalDisponible.DataBind();
                    DropPersonalDisponible.SelectedValue = turno.doctor.id.ToString();
                    verHorariosDelDoctor();
                }
                else
                {
                    validar_permisos("Agregar turnos");
                    turno = new Turno();
                    DropEspecialidad.Visible = false;
                    LabelEspecilidad.Visible = false;
                    TextBoxfecha.Visible = false;
                    TFecha.Style.Add("display","none");
                    DropHora.Visible = false;
                    LabelHora.Visible = false;
                    DropPersonalDisponible.Visible = false;
                    LabelPersonal.Visible = false;
                    
                }
            }
        }
        protected void TextBoxDni_TextChanged(object sender, EventArgs e)
        {
            if (validar_dni()) {
                DropEspecialidad.Visible = true;
                LabelEspecilidad.Visible = true;
            }
        }
        protected void DropEspecialidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextBoxfecha.Text = "";
            DropHora.Visible = false;
            LabelHora.Visible = false;
            int id = Convert.ToInt32(DropEspecialidad.SelectedValue);
            turno.especialidad = new Especialidad();
            turno.especialidad.id = Convert.ToInt32(DropEspecialidad.SelectedValue);

            DoctorNegocio docNego = new DoctorNegocio();
            doctoresList = new List<Doctor>();
            doctoresList = docNego.listar("especialidades con turno disponible", id);

            DropPersonalDisponible.DataSource = doctoresList;
            DropPersonalDisponible.DataValueField = "id";
            DropPersonalDisponible.DataTextField = "nombreCompleto";
            DropPersonalDisponible.DataBind();
            DropPersonalDisponible.Items.Insert(0, new ListItem("[Seleccionar]", "0"));
            
            DropPersonalDisponible.Visible = true;
            LabelPersonal.Visible = true;
        }

        protected void DropPersonalDisponible_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextBoxfecha.Text = "";
            DropHora.Visible = false;
            LabelHora.Visible = false;

            turno.doctor = new Doctor();
            turno.doctor.id = Convert.ToInt32(DropPersonalDisponible.SelectedValue);
            TFecha.Style.Add("display", "block");
            TextBoxfecha.Visible = true;

            verHorariosDelDoctor();
        }
        protected void TextBoxfecha_TextChanged(object sender, EventArgs e)
        {

                int[] horariosDisponibles = horarios_disponibles();

                DropHora.Visible = true;
                LabelHora.Visible = true;
                DropHora.DataSource = horariosDisponibles;
                DropHora.DataBind();
                if (horariosDisponibles.Count() < 1)
                {
                    LabelSinHorario.Text = "*No hay horarios disponibles en ese dia";
                }
                else
                {
                    LabelSinHorario.Text = "";
                }
        }

        protected void ButtonReservar_Click(object sender, EventArgs e)
        {
            if (DropHora.SelectedValue.Trim() != "")
            {
                if (validar_fecha())
                {
                    Empleado emp = new Empleado();
                    emp = (Empleado)Session["empleado"];
                    turno.empleado = new Secretaria();
                    turno.empleado.idEmpleado = emp.idEmpleado;
                    turno.hora = Convert.ToInt32(DropHora.SelectedValue);
                    EstadoTurnoNegocio estNego = new EstadoTurnoNegocio();
                    string accion = "";
                    if (turno.id != 0)
                    {
                        turno.estado = new EstadoTurno();
                        turno.estado.id = estNego.listar("Modificado");
                        TurnoNegocio turNego = new TurnoNegocio();
                        turNego.modificar(turno);
                        accion = "modificado";
                        enviar_mail_confirmacion(turno);
                    }
                    else
                    {
                        turno.estado = new EstadoTurno();
                        turno.estado.id = estNego.listar("Esperando");
                        TurnoNegocio turNego = new TurnoNegocio();
                        turNego.agregar(turno);
                        accion = "agregado";
                        enviar_mail_confirmacion(turno);
                    }
                    Response.Redirect("inicio.aspx?accion=" + accion);
                }
            }    
        }


        public bool validar_fecha()
        {
            LabelSinHorario.Text = "";
            bool validar = true;
            DateTime fechaActual = DateTime.Now;

            string anio = Convert.ToDateTime(TextBoxfecha.Text).ToString("yyyy");
            string mes = Convert.ToDateTime(TextBoxfecha.Text).ToString("MM");
            string dia = Convert.ToDateTime(TextBoxfecha.Text).ToString("dd");
            int hora = Convert.ToInt32(DropHora.SelectedValue);
            string minutos = Convert.ToDateTime(TextBoxfecha.Text).ToString("mm");
            string segundos = Convert.ToDateTime(TextBoxfecha.Text).ToString("ss");
            
            DateTime fecha = new DateTime(Convert.ToInt32(anio), Convert.ToInt32(mes), Convert.ToInt32(dia), hora, Convert.ToInt32(minutos), Convert.ToInt32(segundos));

            if (fechaActual > fecha)
            {
                LabelSinHorario.Text += "*La fecha o hora del turno no puede ser menor a la actual";
                validar = false;
            }
            return validar;
        }


        public bool validar_dni()
        {
            LabelError.Text = "";
            PacienteNegocio pacNego = new PacienteNegocio();
            paciente = new Paciente();
            paciente = pacNego.buscar("dni", TextBoxDni.Text);
            EmpleadoNegocio empNego = new EmpleadoNegocio();
            Empleado emp = new Empleado();
            emp = empNego.buscar(TextBoxDni.Text);
            if (paciente.id != 0)
            {
                if(turno == null) turno = new Turno();
                turno.paciente = new Paciente();
                turno.paciente.id = paciente.id;
                return true;
            }
            if (emp.idEmpleado != 0)
            {
                LabelError.Text = "*Error el DNI ingresado pertenece a un empleado";
                return false;
            }
            Response.Redirect("accionesPaciente.aspx?noRegistrado=" + "noRegistrado");
            return false;
            
        }

        public int[] horarios_disponibles()
        {
            turno.fecha = Convert.ToDateTime(TextBoxfecha.Text);
            TurnoNegocio turNego = new TurnoNegocio();
            turnosList = turNego.turnos_medico_especialidad_fecha(turno);

            string dia = turno.fecha.ToString("dddd");
            HorarioNegocio horNego = new HorarioNegocio();
            doctoresList2 = horNego.horarios_doctor_especialidad("con dia",turno.doctor.id, turno.especialidad.id,dia);

            int cont = 0;
            foreach (var doctor in doctoresList2)
            {
                for (int i = doctor.horario.horaInicio; i < doctor.horario.horaFin; i++)
                {
                    cont++;
                    foreach (var turno in turnosList)
                    {
                        if (turno.hora == i)
                        {
                            cont--;
                        }
                    }
                }
            }
            if (horaEditando != 99)
            {
                cont++;
            }
            int[] horariosDisponibles = new int[cont];

            cont = 0;
            bool b = false;
            foreach (var doctor in doctoresList2)
            {
                for (int i = doctor.horario.horaInicio; i < doctor.horario.horaFin; i++)
                {
                    foreach (var turno in turnosList)
                    {
                        if (turno.hora == i)
                        {
                            b = true;
                            if (horaEditando == turno.hora)
                            {
                                horariosDisponibles[cont] = horaEditando;
                                cont++;
                            }
                        }
                    }
                    if (b == false)
                    {
                        horariosDisponibles[cont] = i;
                        cont++;
                    }
                    b = false;
                }
            }
            return horariosDisponibles;
        }

       public void verHorariosDelDoctor()
        {
            labelHorariosDoctor.Text = "";
            HorarioNegocio horNegg = new HorarioNegocio();
            List<Doctor>  horariosDiasTrabajo = new List<Doctor>();
            horariosDiasTrabajo = horNegg.horarios_doctor_especialidad("sin dia", turno.doctor.id, turno.especialidad.id, "");
            string[] horarioDias=new string[7];
            bool[] banderaDias = new bool[7];
            foreach (var horario in horariosDiasTrabajo)
            {
                switch (horario.horario.dias)
                {
                    case "lunes":
                        if (banderaDias[0]==false) {
                            horarioDias[0] = "<p class='text-light fw-bold mb-0 mt-1'>>Lunes: </p>";
                            banderaDias[0] = true;
                        }
                        horarioDias[0] += "  -De " + horario.horario.horaInicio.ToString() + ":00Hs a " + horario.horario.horaFin.ToString() + ":00hs </br>";
                        break;
                    case "martes":
                        if (banderaDias[1] == false)
                        {
                            horarioDias[1] = "<p class='text-light fw-bold mb-0 mt-1'>>Martes: </p>";
                            banderaDias[1] = true;
                        }
                        horarioDias[1] += "  -De " + horario.horario.horaInicio.ToString() + ":00Hs a " + horario.horario.horaFin.ToString() + ":00hs </br>";
                        break;
                    case "miércoles":
                        if (banderaDias[2] == false)
                        {
                            horarioDias[2] = "<p class='text-light fw-bold mb-0 mt-1'>>Miercoles: </p>";
                            banderaDias[2] = true;
                        }
                        horarioDias[2] += "  -De " + horario.horario.horaInicio.ToString() + ":00Hs a " + horario.horario.horaFin.ToString() + ":00hs </br>";
                        break;
                    case "jueves":
                        if (banderaDias[3] == false)
                        {
                            horarioDias[3] = "<p class='text-light fw-bold mb-0 mt-1'>>Jueves: </p>";
                            banderaDias[3] = true;
                        }
                        horarioDias[3] += "  -De " + horario.horario.horaInicio.ToString() + ":00Hs a " + horario.horario.horaFin.ToString() + ":00hs </br>";
                        break;
                    case "viernes":
                        if (banderaDias[4] == false)
                        {
                            horarioDias[4] = "<p class='text-light fw-bold mb-0 mt-1'>>Viernes: </p>";
                            banderaDias[4] = true;
                        }
                        horarioDias[4] += "  -De " + horario.horario.horaInicio.ToString() + ":00Hs a " + horario.horario.horaFin.ToString() + ":00hs </br>";
                        break;
                    case "sábado":
                        if (banderaDias[5] == false)
                        {
                            horarioDias[5] = "<p class='text-light fw-bold mb-0 mt-1'>>Sabado: </p>";
                            banderaDias[5] = true;
                        }
                        horarioDias[5] += "  -De " + horario.horario.horaInicio.ToString() + ":00Hs a " + horario.horario.horaFin.ToString() + ":00hs </br>";
                        break;
                    case "domingo":
                        if (banderaDias[6] == false)
                        {
                            horarioDias[6] = "<p class='text-light fw-bold mb-0 mt-1'>>Domingo: </p>";
                            banderaDias[6] = true;
                        }
                        horarioDias[6] += "  -De " + horario.horario.horaInicio.ToString() + ":00Hs a " + horario.horario.horaFin.ToString() + ":00hs </br>";
                        break;
                }

            }
            for (int i=0;i<7;i++)
            {
                labelHorariosDoctor.Text += horarioDias[i];
            }
            horariosDoctorContainer.Style.Add("display","block");
        }

        public void validar_permisos(string val)
        {
            permisosList = new List<Permiso>();
            permisosList = (List<Permiso>)Session["permisos"];
            ValidarPermiso valPer = new ValidarPermiso();
            if (permisosList == null || valPer.validar_permiso(permisosList, val) == false)
            {
                Response.Redirect("inicio.aspx");
            }
        }

        public void enviar_mail_confirmacion(Turno turnoEnviar)
        {
            PacienteNegocio pacNego = new PacienteNegocio();
            Paciente pacienteMail = new Paciente();
            pacienteMail = pacNego.buscar("id", turno.paciente.id.ToString());
            Email email = new Email();
            string fecha = turno.fecha.ToString("dd/MM/yyyy");
            string hora = turno.hora.ToString();
            hora += ":00 Hs";
            email.correo(pacienteMail.email,"Turno Reservado",fecha,hora);
            email.enviar_email();
        }

    }
}