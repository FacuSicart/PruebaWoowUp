using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace PruebaWoowUp
{
    public enum TipoAlerta
    {
        Informativa,
        Urgente
    }

    public abstract class Alerta
    {
        public int Id { get; set; }
        public string Mensaje { get; set; } 
        public DateTime FechaExpiracion { get; set; } 
        public TipoAlerta Tipo { get; set; }
        public bool Leida { get; set; }
        public int? UsuarioDestinoId { get; set; }
        public int? TemaId { get; set; }
        public abstract void Enviar();
    }
    //Herencia de la clase abstracta Alerta para aplicar polimorfismo sobre el metodo Enviar
    public class AlertaInformativa : Alerta
    {
        public override void Enviar()
        {
            Console.WriteLine($"Se envio la alerta informativa a {(UsuarioDestinoId.HasValue ? "usuario " + UsuarioDestinoId.Value.ToString() : "todos los usuarios")}.");
        }
    }
    //Herencia de la clase abstracta Alerta para aplicar polimorfismo sobre el metodo Enviar
    public class AlertaUrgente : Alerta
    {
        public override void Enviar()
        {
            Console.WriteLine($"Se envio la alerta urgente a {(UsuarioDestinoId.HasValue ? "usuario " + UsuarioDestinoId.Value.ToString() : "todos los usuarios")}.");
        }
    }

    public class Usuario
    {
        public int Id { get; set; } 
        public string Nombre { get; set; }
        public List<int> TemasSubscritos { get; set; } = new List<int>(); 

        //Metodo para seleccionar temas a suscribirse
        public void SuscribirseATema(int temaId)
        {
            TemasSubscritos.Add(temaId);
        }
        //Metodo para ver temas suscriptos
        public void MostrarTemasSuscritos()
        {
            if (TemasSubscritos.Count == 0)
            {
                Console.WriteLine("El usuario no está suscrito a ningún tema.");
                return;
            }

            Console.WriteLine("Temas a los que está suscrito el usuario:");
            foreach (var temaId in TemasSubscritos)
            {
                var tema = SistemaAlertas.ObtenerInstancia().ObtenerTemaPorId(temaId);
                if (tema != null)
                {
                    Console.WriteLine($"ID: {tema.Id}, Nombre: {tema.Nombre}");
                }
                else
                {
                    Console.WriteLine($"El tema con ID {temaId} no existe.");
                }
            }
        }
    }

    public class Tema
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class SistemaAlertas
    {
        private static SistemaAlertas instancia; 
        public List<Usuario> usuarios = new List<Usuario>();
        public List<Tema> temas = new List<Tema>(); 
        public List<Alerta> alertas = new List<Alerta>();

        private SistemaAlertas() { }
        public static SistemaAlertas ObtenerInstancia()
        {
            if (instancia == null) //Si no existe una instancia previa, se crea una nueva
            {
                instancia = new SistemaAlertas();
            }
            return instancia; //Se retorna la instancia existente o recien creada
        }

        // Metodo para registrar un usuario nuevo
        public void RegistrarUsuario(string nombre)
        {
            var usuario = new Usuario
            {
                Id = usuarios.Count + 1,
                Nombre = nombre
            };
            usuarios.Add(usuario);
            Console.WriteLine($"El usuario '{nombre}' se registro correctamente.");
        }

        //Metodo para registrar un tema nuevo
        public void RegistrarTema(string nombre)
        {
            var tema = new Tema
            {
                Id = temas.Count + 1,
                Nombre = nombre
            };
            temas.Add(tema);
            Console.WriteLine($"El tema '{nombre}' se registro correctamente.");
        }
        //Metodo para obter el tema por id
        public Tema ObtenerTemaPorId(int temaId)
        {
            return temas.FirstOrDefault(t => t.Id == temaId);
        }

        //Metodo para mostrar los temas 
        public void MostrarTemas()
        {
            foreach (var tema in temas)
            {
                Console.WriteLine($"ID: {tema.Id}, Nombre: {tema.Nombre}");
            }
        }
        //Metodo para mostrar los usuarios
        public void MostrarUsuarios()
        {
            foreach (var usuario in usuarios)
            {
                Console.WriteLine($"ID: {usuario.Id}, Nombre: {usuario.Nombre}");
            }
        }
        //Metodo para enviar alerta a todos los que esten suscriptos a un tema
        public void EnviarAlerta(int temaId, Alerta alerta)
        {
            var usuariosSuscritos = usuarios.Where(u => u.TemasSubscritos.Contains(temaId)).ToList();

            foreach (var usuario in usuariosSuscritos)
            {
                var alertaParaUsuario = alerta.GetType().GetConstructor(Type.EmptyTypes).Invoke(null) as Alerta;
                alertaParaUsuario.Id = alerta.Id;
                alertaParaUsuario.Mensaje = alerta.Mensaje;
                alertaParaUsuario.FechaExpiracion = alerta.FechaExpiracion;
                alertaParaUsuario.Tipo = alerta.Tipo;
                alertaParaUsuario.Leida = false;
                alertaParaUsuario.UsuarioDestinoId = usuario.Id;
                alertaParaUsuario.TemaId = temaId;

                alertaParaUsuario.Enviar();
                alertas.Add(alertaParaUsuario);
            }
        }
        //Metodo para enviar alerta a un usuario solo
        public void EnviarAlertaUnica(int usuarioId, int temaId, Alerta alerta)
        {
            var alertaParaUsuario = alerta.GetType().GetConstructor(Type.EmptyTypes).Invoke(null) as Alerta;
            alertaParaUsuario.Id = alerta.Id;
            alertaParaUsuario.Mensaje = alerta.Mensaje;
            alertaParaUsuario.FechaExpiracion = alerta.FechaExpiracion;
            alertaParaUsuario.Tipo = alerta.Tipo;
            alertaParaUsuario.Leida = false;
            alertaParaUsuario.UsuarioDestinoId = usuarioId;
            alertaParaUsuario.TemaId = temaId;

            alertaParaUsuario.Enviar();
            alertas.Add(alertaParaUsuario);

        }
        //Metodo que muestra alertas no leidas o por tema
        public void MostrarAlertas(int usuarioId, int numDecision, int? temaId = null)
        {
            List<Alerta> alertasUsuario = new List<Alerta>();
            //Depende la opción que se elija se se muestra la que no esta leida o el tema seleccionado
            if (numDecision == 6)
            {
                alertasUsuario = alertas.Where(a => a.UsuarioDestinoId == usuarioId && a.Leida == false).ToList();
            }
            else if (numDecision == 7)
            {
                alertasUsuario = alertas.Where(a => a.UsuarioDestinoId == usuarioId && a.TemaId == temaId).ToList();
            }

            if (alertasUsuario.Count == 0)
            {
                Console.WriteLine($"El usuario {usuarioId} no tiene alertas.");
                return;
            }
            //Se separa las alertas urgentes e informativas en listas diferentes
            var alertasUrgentes = alertasUsuario.Where(a => a.Tipo == TipoAlerta.Urgente).ToList();
            var alertasInformativas = alertasUsuario.Where(a => a.Tipo == TipoAlerta.Informativa).ToList();

            //Invierte el orden de la lista de alertas urgentes de este modo se muestran primero las últimas en llegar
            alertasUrgentes.Reverse();

            //Combina las listas teniendo primero las urgentes y luego las informativas
            alertasUsuario = alertasUrgentes.Concat(alertasInformativas).ToList();

            Console.WriteLine($"Alertas del usuario {usuarioId}:");
            foreach (var alerta in alertasUsuario)
            {
                Console.WriteLine($"ID: {alerta.Id}, Mensaje: {alerta.Mensaje}, Fecha de expiración: {alerta.FechaExpiracion}, Tipo: {alerta.Tipo}, Leída: {(alerta.Leida ? "Sí" : "No")}");
            }
        }

        //Metodo para marcar una alerta como leída por un usuario específico
        public void MarcarAlertaComoLeida(int alertaId, int usuarioId)
        {
            var alerta = alertas.FirstOrDefault(a => a.Id == alertaId && a.UsuarioDestinoId == usuarioId);
            if (alerta != null)
            {
                alerta.Leida = true;
                Console.WriteLine($"La alerta {alertaId} se marco como leída por el usuario {usuarioId}.");
            }
            else
            {
                Console.WriteLine($"La alerta {alertaId} no existe o no es para el usuario {usuarioId}.");
            }
        }
        //Metodo para obtener el usuario por id
        public Usuario ObtenerUsuarioPorId(int idUsuario)
        {
            return usuarios.FirstOrDefault(u => u.Id == idUsuario);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var sistemaAlertas = SistemaAlertas.ObtenerInstancia();

            Console.WriteLine("Bienvenido al Sistema de Alertas");

            while (true)
            {
                Console.WriteLine("\nMenú:");
                Console.WriteLine("1. Registrar usuario");
                Console.WriteLine("2. Registrar tema");
                Console.WriteLine("3. Agregar tema de interes a usuario");
                Console.WriteLine("4. Enviar alerta a todos los que tengan el tema");
                Console.WriteLine("5. Enviar alerta a un usuario");
                Console.WriteLine("6. Mostrar alertas de un usuario que aun no leyo");
                Console.WriteLine("7. Mostrar alertas de un usuario por tema");
                Console.WriteLine("8. Marcar alerta como leída");
                Console.WriteLine("0. Salir");
                Console.Write("Seleccione una opción: ");
                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        Console.Write("Ingrese el nombre del usuario: ");
                        string nombreUsuario = Console.ReadLine();
                        sistemaAlertas.RegistrarUsuario(nombreUsuario);
                        break;
                    case "2":
                        Console.Write("Ingrese el nombre del tema: ");
                        string nombreTema = Console.ReadLine();
                        sistemaAlertas.RegistrarTema(nombreTema);
                        break;
                    case "3":
                        sistemaAlertas.MostrarUsuarios();
                        Console.Write("Ingrese el ID del usuario: ");
                        int idUsuario = int.Parse(Console.ReadLine());
                        sistemaAlertas.MostrarTemas();
                        Console.Write("Ingrese el ID del tema: ");
                        int idTema = int.Parse(Console.ReadLine());

                        Usuario usuario = sistemaAlertas.ObtenerUsuarioPorId(idUsuario);
                        if (usuario != null)
                        {
                            usuario.SuscribirseATema(idTema);
                        }
                        else
                        {
                            Console.WriteLine("El usuario seleccionado no existe.");
                        }
                        break;
                    case "4":
                        sistemaAlertas.MostrarTemas();
                        Console.Write("Ingrese el ID del tema: ");
                        idTema = int.Parse(Console.ReadLine());
                        Console.Write("Ingrese el mensaje de la alerta: ");
                        string mensajeAlerta = Console.ReadLine();
                        Console.Write("Ingrese la fecha de expiración (YYYY-MM-DD HH:MM): ");
                        DateTime fechaExpiracion = DateTime.Parse(Console.ReadLine());
                        Console.Write("Ingrese el tipo de alerta (Informativa/Urgente): ");
                        TipoAlerta tipoAlerta = (TipoAlerta)Enum.Parse(typeof(TipoAlerta), Console.ReadLine(), true);

                        Alerta nuevaAlerta;
                        if (tipoAlerta == TipoAlerta.Informativa)
                        {
                            nuevaAlerta = new AlertaInformativa();
                        }
                        else
                        {
                            nuevaAlerta = new AlertaUrgente();
                        }

                        nuevaAlerta.Mensaje = mensajeAlerta;
                        nuevaAlerta.FechaExpiracion = fechaExpiracion;
                        nuevaAlerta.Tipo = tipoAlerta;

                        sistemaAlertas.EnviarAlerta(idTema, nuevaAlerta);
                        break;
                    case "5":
                        sistemaAlertas.MostrarTemas();
                        Console.Write("Ingrese el ID del tema: ");
                        idTema = int.Parse(Console.ReadLine());
                        Console.Write("Ingrese el ID del usurio: ");
                        idUsuario = int.Parse(Console.ReadLine());
                        Console.Write("Ingrese el mensaje de la alerta: ");
                        mensajeAlerta = Console.ReadLine();
                        Console.Write("Ingrese la fecha de expiración (YYYY-MM-DD HH:MM): ");
                        fechaExpiracion = DateTime.Parse(Console.ReadLine());
                        Console.Write("Ingrese el tipo de alerta (Informativa/Urgente): ");
                        tipoAlerta = (TipoAlerta)Enum.Parse(typeof(TipoAlerta), Console.ReadLine(), true);

                        Alerta Alerta;
                        if (tipoAlerta == TipoAlerta.Informativa)
                        {
                            nuevaAlerta = new AlertaInformativa();
                        }
                        else
                        {
                            nuevaAlerta = new AlertaUrgente();
                        }

                        nuevaAlerta.Mensaje = mensajeAlerta;
                        nuevaAlerta.FechaExpiracion = fechaExpiracion;
                        nuevaAlerta.Tipo = tipoAlerta;

                        sistemaAlertas.EnviarAlertaUnica(idUsuario, idTema, nuevaAlerta);
                        break;
                    case "6":
                        Console.WriteLine("IDs los usuarios:");
                        sistemaAlertas.MostrarUsuarios();

                        //Solicita al usuario que seleccione un ID de usuario
                        Console.Write("Ingrese el ID del usuario para ver sus alertas: ");
                        int usuarioSeleccionado = int.Parse(Console.ReadLine());


                        if (usuarioSeleccionado != null)
                        {
                            //Muestra las alertas del usuario seleccionado
                            SistemaAlertas.ObtenerInstancia().MostrarAlertas(usuarioSeleccionado, 6);
                        }
                        else
                        {
                            Console.WriteLine("El usuario seleccionado no existe.");
                        }
                        break;
                    case "7":
                        Console.WriteLine("IDs los usuarios:");
                        sistemaAlertas.MostrarUsuarios();

                        //Solicita al usuario que seleccione un ID de usuario
                        Console.Write("Ingrese el ID del usuario para ver sus alertas: ");
                        usuarioSeleccionado = int.Parse(Console.ReadLine());

                        if (usuarioSeleccionado != null)
                        {
                            //Muestra las alertas del usuario seleccionado por tema
                            sistemaAlertas.MostrarTemas();
                            Console.Write("Ingrese el ID del tema: ");
                            int temaSeleccionado = int.Parse(Console.ReadLine());
                            sistemaAlertas.MostrarAlertas(usuarioSeleccionado, 7, temaSeleccionado);
                        }
                        else
                        {
                            Console.WriteLine("El usuario seleccionado no existe.");
                        }
                        break;
                    case "8":
                        Console.WriteLine("IDs los usuarios:");
                        sistemaAlertas.MostrarUsuarios();

                        //Solicita al usuario que seleccione un ID de usuario
                        Console.Write("Ingrese el ID del usuario para ver sus alertas: ");
                        usuarioSeleccionado = int.Parse(Console.ReadLine());

                        if (usuarioSeleccionado != null)
                        {
                            //Muestra las alertas del usuario seleccionado
                            SistemaAlertas.ObtenerInstancia().MostrarAlertas(usuarioSeleccionado, 6);
                        }
                        else
                        {
                            Console.WriteLine("El usuario seleccionado no existe.");
                        }
                        Console.Write("Ingrese el ID de la alerta: ");
                        int idAlerta = int.Parse(Console.ReadLine());
                        sistemaAlertas.MarcarAlertaComoLeida(idAlerta, usuarioSeleccionado);
                        break;
                    case "0":
                        Console.WriteLine("Saliendo del programa...");
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intentelo de nuevo.");
                        break;
                }
            }
        }
    }

    [TestClass]
    public class SistemaAlertasTests
    {
        [TestMethod]
        public void TestRegistrarUsuario()
        {
            
            //Arrange
            var sistemaAlertas = SistemaAlertas.ObtenerInstancia();
            var usuario = new Usuario
            {
                Id = sistemaAlertas.usuarios.Count + 1,
                Nombre = "Facu"
            };
            sistemaAlertas.usuarios.Add(usuario);
            //Act
            sistemaAlertas.RegistrarUsuario("Usuario de prueba");

            //Assert
            Assert.AreEqual(1, sistemaAlertas.usuarios.Count());
            Assert.AreEqual("Usuario de prueba", sistemaAlertas.usuarios[0].Nombre);
        }

        [TestMethod]
        public void TestRegistrarTema()
        {
            //Arrange
            var sistemaAlertas = SistemaAlertas.ObtenerInstancia();
            var tema = new Tema
            {
                Id = sistemaAlertas.temas.Count + 1,
                Nombre = "Comida"
            };
            sistemaAlertas.temas.Add(tema);
            //Act
            sistemaAlertas.RegistrarTema("Tema de prueba");

            //Assert
            Assert.AreEqual(1, sistemaAlertas.temas.Count);
            Assert.AreEqual("Tema de prueba", sistemaAlertas.temas[0].Nombre);
        }

        [TestMethod]
        public void TestSuscribirseATema()
        {
            //Arrange
            var sistemaAlertas = SistemaAlertas.ObtenerInstancia();
            sistemaAlertas.RegistrarUsuario("Usuario de prueba");
            sistemaAlertas.RegistrarTema("Tema de prueba");
            var usuario = sistemaAlertas.usuarios[0];

            //Act
            usuario.SuscribirseATema(1);

            //Assert
            Assert.AreEqual(1, usuario.TemasSubscritos.Count);
            Assert.AreEqual(1, usuario.TemasSubscritos[0]);
        }
    }
}
