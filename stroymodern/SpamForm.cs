using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stroymodern
{
    public partial class SpamForm : Form
    {
        private readonly CheckUser _user;
        public SpamForm(CheckUser user)
        {
            _user = user;
            InitializeComponent();
        }

        private void SpamForm_Load(object sender, EventArgs e)
        {
            label2.Text = $"{_user.Login}:{_user.Status}";
        }   

        private void button7_Click(object sender, EventArgs e)
        {
            GoodsForm goods = new GoodsForm(_user);
            goods.Show();
            this.Close();
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            string smtpServer = "smtp.mail.ru";
            int smtpPort = 587;

            string smtpUsername = "picolinyaontop@mail.ru";
            string smtpPassword = "HS8SFhNwnrdGbZBuhHB8";

            using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add(textBox2.Text);
                    mailMessage.Subject = textBox1.Text;
                    mailMessage.Body = textBox3.Text;

                    try
                    {
                        smtpClient.Send(mailMessage);
                        MessageBox.Show("Сообщение отправлено");
                        GoodsForm goods = new GoodsForm(_user);
                        goods.Show();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Сообщение не отправлено {ex.Message}");
                    }
                }
            }
        }
    }
}
