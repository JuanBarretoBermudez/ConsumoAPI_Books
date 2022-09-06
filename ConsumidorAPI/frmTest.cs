using ConsumidorAPI.ApiHelper;
using ConsumidorAPI.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;

namespace ConsumidorAPI
{
    public partial class frmTest : Form
    {

        public frmTest()
        {
            InitializeComponent();
            groupBox1.Visible = false;
            groupBox2.Visible = false;

        }

        public async void btnGet_Click(object sender, EventArgs e)
        {
            //Creamos el listado de Posts a llenar
            List<Post> listado = new List<Post>();
            //Instanciamos un objeto Reply
            Reply oReply = new Reply();
            //poblamos el objeto con el método generic Execute
            oReply = await Consumer.Execute<List<Post>>(this.txtUri.Text, ApiHelper.methodHttp.GET, listado);
            List<Post> listadoReply = (List<Post>)oReply.Data;
            setlistBooks(listadoReply);
            //Poblamos el datagridview
            this.dgvGet.DataSource = oReply.Data;
            //Mostramos el statuscode devuelto, podemos añadirle lógica de validación
            MessageBox.Show("Documentos cargados correctamente  " + oReply.StatusCode);
        }

        public List<Post> setlistBooks(List<Post> listadoReply) 
        {

            List<Post> listadoBooks = setlistBook(listadoReply);
            return listadoReply;
        }

        public async void btnPost_Click(object sender, EventArgs e)
        {
            List<Post> listadoReply = new List<Post>();

            Post post = new Post()

            {
                userId = 101,
                title = "Juan David",
                body = "Haciendo pruebas como ..."

            };

            Reply oReply = new Reply();
            
            oReply = await Consumer.Execute<Post>(this.txtUriPost.Text, ApiHelper.methodHttp.POST, post);

            MessageBox.Show(oReply.StatusCode);
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            Reply oReply = new Reply();

            oReply = await Consumer.Execute<Post>($"{this.txtUriPost.Text}/{this.txtId.Value}", ApiHelper.methodHttp.DELETE, null);

            MessageBox.Show(oReply.StatusCode);
        }

        public List<Post> setlistBook(List<Post> listadoReply)
        {
            string Query = string.Empty;
            string queryBooks = string.Empty;

            string connectionstring = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;


            List<Post> posts = new List<Post>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    foreach (Post books in listadoReply)
                    {
                        
                        queryBooks = @"insert into dbo.libros(Id, UserId, Title, Body) Values(" + books.id + ", '" + books.userId + "', '" + books.title + "', '" + books.body + "');";

                        con.Open();
                        posts = con.Query<Post>(queryBooks).ToList();

                        con.Close();
                    }
                }
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            return posts;
        }

    }
}
