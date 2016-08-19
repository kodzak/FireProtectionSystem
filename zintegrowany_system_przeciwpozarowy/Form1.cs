using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using SKYPE4COMLib;
using Twilio;
using zintegrowany_system_przeciwpozarowy.Model;

//using SkypeNet.Lib.Core.Objects;

namespace zintegrowany_system_przeciwpozarowy
{
    public partial class Form1 : Form
    {
        private readonly Owner owner = new Owner();
        private Owner selectedOwner;

        public Form1()
        {
            InitializeComponent();
            selectedOwner = null;
            var cancellationTokenSource = new CancellationTokenSource();
            var task = Repeat.Interval(
                TimeSpan.FromSeconds(5),
                () => CheckSMS(), cancellationTokenSource.Token);
        }

        //początkowe dane do bazy
        //Owner owners = new Owner();
        //owners.Id = 1;
        //owners.Name = "Konrad";
        //owners.Surname = "Kochaniak";
        //owners.Address = "Komandorska 4, Częstochowa";
        //owners.OwnerNumber = "+48661045723";
        //context.Owner.Add(owners);
        //Sensor sensors = new Sensor();
        //sensors.SensorNumber = "+48661045723";
        //sensors.OwnerId = 1;
        //context.Sensors.Add(sensors);
        // var events = new Event();
        // events.Id = 1;
        // events.Date = DateTime.Now;
        //// events.IdSensor=context.Sensors;
        // events.RealEvent = true;
        // context.Events.Add(events);
        // context.SaveChanges();
        private void CheckSMS()
        {
            var AccountSid = "AC9490deba5ac4a07296bde073d511e957";
            var AuthToken = "5fdbed4bd6fea00c4a39364aeff353e7";
            var twilio = new TwilioRestClient(AccountSid, AuthToken);

            // Build the parameters 
            var options = new MessageListRequest();

            var messages = twilio.ListMessages(options);

            foreach (var message in messages.Messages)
            {
                using (var context = new EdContext())
                {
                    var sensor = context.Sensors.SingleOrDefault(a => a.SensorNumber == message.From);
                    if (sensor != null)
                    {
                        var owner = context.Owner.SingleOrDefault(p => p.Id == sensor.OwnerId);
                        //wysyłanie wiadomości do właściciela o zdarzeniu
                        var error = true;
                        do
                        {
                            var sendmessage = twilio.SendMessage("+48799448908", owner.OwnerNumber,
                                "Pożar w twoim domu! " + owner.Address);
                            error = sendmessage.ErrorCode.HasValue;

                        } while (error);
                        dataGridView1.Invoke(new Action(() => ownerBindingSource.Add(owner)));
                        
                        dataGridView1.DefaultCellStyle.BackColor = Color.Red;
                        dataGridView1.DefaultCellStyle.ForeColor = Color.White;
                    }
                }

                twilio.DeleteMessage(message.Sid);
            }
        }
     
        private void SaveLog(string action, string what, string whatId)
        {
            using (var context = new EdContext())
            {
                Logs logs = new Logs();
                logs.Date = DateTime.Now;
                logs.Action = action;
                logs.What = what;
                logs.WhatId = whatId;
                context.Logs.Add(logs);
                context.SaveChanges();
                dataGridView6.Update();
            }
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "zadzwon")
            {
                //Skype skype;
                //skype = new Skype();
                //owner.OwnerNumber = "+48661045723";
                //var SkypeID = owner.OwnerNumber;
                //var call = skype.PlaceCall(SkypeID);
                //do
                //{
                //    Thread.Sleep(1);
                //} while (call.Status != TCallStatus.clsInProgress);
                //call.StartVideoSend();
                owner.OwnerNumber = "+485553223";
                owner.Id = 31;
                SaveLog("Zadzwoń", owner.OwnerNumber, owner.Id.ToString());
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "SendHelp")
            {
                using (var context = new EdContext())
                {

                   var sensor = context.Sensors.SingleOrDefault(a => a.Id == owner.Id);
                    var events = new Event();
                    events.Date = DateTime.Now;
                    events.IdSensor = 2;
                    events.RealEvent = true;
                    context.Events.Add(events);
                    context.SaveChanges();
                    owner.Address = "Lesna 4, Częstochowa";
                    owner.Id = 31;
                    SaveLog("Wyślij pomoc", owner.Address, owner.Id.ToString());
                }
               
            }
        }



        public void GetEvents()
        {
            EdContext context = new EdContext();
            var result = (from ob1 in context.Owner
                join ob2 in context.Events
                    on ob1.Id equals ob2.IdSensor
                join ob3 in context.Sensors
                    on ob2.IdSensor equals ob3.Id
                select new
                {
                    ob1.Address,
                    ob2.Date,
                    ob2.RealEvent,
                    ob3.Place
                }).ToList();

            dataGridView3.DataSource = result;
            dataGridView3.Columns[0].HeaderText = "Adres";
            dataGridView3.Columns[1].HeaderText = "Data zdarzenia";
            dataGridView3.Columns[2].HeaderText = "Pożar";
            dataGridView3.Columns[3].HeaderText = "Mijesce rozmieszczenia";
            dataGridView3.Refresh();

        }

        public void GetMaintenance()
        {
            EdContext context = new EdContext();
            var result = (from ob1 in context.Owner
                join ob2 in context.Maintenances
                    on ob1.Id equals ob2.IdSensor
                join ob3 in context.Sensors
                    on ob1.Id equals ob3.OwnerId
                select new
                {
                    ob1.Address,
                    ob1.OwnerNumber,
                    ob2.DataConservation,
                    ob2.DataNextConservation,
                    ob2.Work,
                    ob3.Place
                }).ToList();

            dataGridView4.DataSource = result;
            dataGridView4.Columns[1].HeaderText = "Adres";
            dataGridView4.Columns[2].HeaderText = "Numer właściciela";
            dataGridView4.Columns[3].HeaderText = "Data konserwacji";
            dataGridView4.Columns[4].HeaderText = "Data następnej konserwacji";
            dataGridView4.Columns[5].HeaderText = "Czujnik sprawny";
            dataGridView4.Columns[6].HeaderText = "Miejsce rozmieszczenia";

            dataGridView4.Refresh();

        }

        public void GetIntervention()
        {
            EdContext context = new EdContext();
            var result = (from ob1 in context.Owner
                join ob2 in context.Interventions
                    on ob1.Id equals ob2.IdOwner
                select new
                {
                    ob1.Address,
                    ob1.OwnerNumber,
                    ob2.WhenArrive,
                    ob2.WhenEnd,
                    ob2.WhenLeave,
                    ob2.HowLong,
                    ob2.DeadVictims,
                    ob2.InjureVictims,
                    ob2.Arson,
                    ob2.Description

                }).ToList();
            dataGridView5.DataSource = result;
            dataGridView5.Columns[0].HeaderText = "Adres";
            dataGridView5.Columns[1].HeaderText = "Numer właściciela";
            dataGridView5.Columns[2].HeaderText = "Kiedy przyjechali";
            dataGridView5.Columns[3].HeaderText = "Kiedy skończyli akcje";
            dataGridView5.Columns[4].HeaderText = "Kiedy odjechali";
            dataGridView5.Columns[5].HeaderText = "Jak długo";
            dataGridView5.Columns[6].HeaderText = "Ofriary śmiertelne";
            dataGridView5.Columns[7].HeaderText = "Zranieni";
            dataGridView5.Columns[8].HeaderText = "Podpalenie";
            dataGridView5.Columns[9].HeaderText = "Opis";
            dataGridView5.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'logs1.Logs' table. You can move, or remove it, as needed.
            this.logsTableAdapter.Fill(this.logs1.Logs);
            // TODO: This line of code loads data into the 'maintenances._Maintenances' table. You can move, or remove it, as needed.
            this.maintenancesTableAdapter.Fill(this.maintenances._Maintenances);
            // TODO: This line of code loads data into the 'owner_add.Owners' table. You can move, or remove it, as needed.
            ownersTableAdapter.Fill(owners_add.Owners);
            GetEvents();
            GetMaintenance();
            GetIntervention();

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void tabPage2_Click_1(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void label3_Click_1(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
        }

        private void label7_Click(object sender, EventArgs e)
        {
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        //
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            eventBindingSource.Clear();
            var selectedItem = comboBox2.SelectedItem.ToString();
            var index = selectedItem.IndexOf(":");
            if (index == -1) return;
            var id = selectedItem.Substring(0, index);

            using (var context = new EdContext())
            {
                var person = context.Owner.SingleOrDefault(a => a.Id.ToString() == id);
                if (person == null) return;
                var sensors = context.Sensors.Where(a => a.OwnerId == person.Id).ToList();
                if (!sensors.Any()) return;
                var events = new List<Event>();
                foreach (var sensor in sensors)
                {
                    var sensorEvents = context.Events.Where(a => a.IdSensor == sensor.Id).ToList();
                    events.AddRange(sensorEvents);
                }

                foreach (var item in events)
                {
                    eventBindingSource.Add(item);
                }
            }
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {
        }

        private void label10_Click(object sender, EventArgs e)
        {
        }

        private void label9_Click(object sender, EventArgs e)
        {
        }

        private void label6_Click(object sender, EventArgs e)
        {
        }

        //dodawanie nowego czujnika
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox6.Text)) return;
            if (selectedOwner == null) return;
            int parsedValue;
            if (!int.TryParse(textBox6.Text, out parsedValue))
            {
                MessageBox.Show("Numer czujnika musi składać się z liczb!");
            }
            using (var context = new EdContext())
            {
                if (textBox6.Text.StartsWith("+48"))
                {
                    var sensor = new Sensor
                    {
                        OwnerId = selectedOwner.Id,
                        SensorNumber = textBox6.Text,
                        Place = comboBox5.Text
                    };
                    context.Sensors.Add(sensor);
                    context.SaveChanges();
                    SaveLog("Add Sensor", sensor.SensorNumber, sensor.Id.ToString());
                }
                else
                {
                    var sensor = new Sensor
                    {
                        OwnerId = selectedOwner.Id,
                        SensorNumber = "+48" + textBox6.Text,
                        Place = comboBox5.Text
                    };
                    context.Sensors.Add(sensor);
                    context.SaveChanges();
                    SaveLog("Dodaj czujnik", sensor.SensorNumber, sensor.Id.ToString());
                }

                ClearTextBoxes(Controls);
                comboBox5.ResetText();
            }
        }

        private void ClearTextBoxes(Control.ControlCollection cc)
        {
            foreach (Control ctrl in cc)
            {
                var tb = ctrl as TextBox;
                if (tb != null)
                    tb.Text = "";
                else
                    ClearTextBoxes(ctrl.Controls);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using (var context = new EdContext())
            {
                var owner = new Owner();
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Imie nie może być puste!");
                    return;
                }
                owner.Name = textBox1.Text;
                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("Nazwisko nie może być puste!");
                    return;
                }
                owner.Surname = textBox2.Text;
                if (string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Adres nie może być pusty!");
                    return;
                }
                owner.Address = textBox3.Text;
                int parsedValue;
                if (!int.TryParse(textBox4.Text, out parsedValue))
                {
                    MessageBox.Show("Numer właściciela musi składać się z liczb!");
                    return;
                }
                if (textBox4.Text.StartsWith("+48"))
                {
                    owner.OwnerNumber = textBox4.Text;
                }
                else
                {
                    owner.OwnerNumber = "+48" + textBox4.Text;
                }
                owner.TypeOfBulding = comboBox6.SelectedItem.ToString();

                context.Owner.Add(owner);
                context.SaveChanges();
                SaveLog("Dodaj właściciela", owner.Address, owner.Id.ToString());
                ownersTableAdapter.Fill(owners_add.Owners);
                ClearTextBoxes(Controls);
                comboBox6.ResetText();
            }
        }

        //dodawanie czujników, wybierasz z listy adres dla którego chcesz dodać czujnik a potem do labelów
        //wrzucane są dane właściciela do nich textbox6 zawierać będzie numer właściciela a button2 zatwierdzasz 
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label7.Text = "Imię: ";
            label8.Text = "Nazwisko: ";
            label9.Text = "Adres: ";
            label10.Text = "Numer właściciela: ";
            label12.Text = "Numery czujników: ";

            var selectedItem = comboBox1.SelectedItem.ToString();
            var index = selectedItem.IndexOf(":");
            if (index == -1) return;
            var id = selectedItem.Substring(0, index);

            using (var context = new EdContext())
            {
                var person = context.Owner.SingleOrDefault(a => a.Id.ToString() == id);
                var sensors = context.Sensors.Where(a => a.OwnerId.ToString() == id).ToList();
                if (person == null) return;
                label7.Text += person.Name;
                label8.Text += person.Surname;
                label9.Text += person.Address;
                label10.Text += person.OwnerNumber;

                foreach (var sensor in sensors)
                {
                    label12.Text += " \n " + sensor.SensorNumber + " (" + sensor.Place + ") ";

                }

                selectedOwner = new Owner();
                selectedOwner.Id = person.Id;
                selectedOwner.Name = person.Name;
                selectedOwner.Surname = person.Surname;
                selectedOwner.Address = person.Address;
                selectedOwner.OwnerNumber = person.OwnerNumber;
            }
        }

        private void label5_Click_1(object sender, EventArgs e)
        {
        }

        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView2_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {
        }

        private void comboBox1_Enter(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            using (var context = new EdContext())
            {
                var people = context.Owner.ToList();

                if (!people.Any()) return;

                foreach (var person in people)
                {
                    comboBox1.Items.Add(person.Id + ":" + person.Name + " " + person.Surname);
                }
            }
        }

        private void comboBox2_Enter(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            using (var context = new EdContext())
            {
                var people = context.Owner.ToList();

                if (!people.Any()) return;

                foreach (var person in people)
                {
                    comboBox2.Items.Add(person.Id + ":" + person.Address);
                }
            }
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void label7_Click_1(object sender, EventArgs e)
        {
        }

        private void eventBindingSource_CurrentChanged(object sender, EventArgs e)
        {
        }

        private void label12_Click(object sender, EventArgs e)
        {
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

        }

        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView4.Columns[e.ColumnIndex].Name == "czadzwon")
            {
                Skype skype;
                skype = new Skype();

                var SkypeID = owner.OwnerNumber;
                var call = skype.PlaceCall(SkypeID);
                do
                {
                    Thread.Sleep(1);
                } while (call.Status != TCallStatus.clsInProgress);
                call.StartVideoSend();
                SaveLog("Zadzwoń", owner.OwnerNumber, owner.Id.ToString());
            }

        }

        private void comboBox3_Enter(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            using (var context = new EdContext())
            {
                var people = context.Owner.ToList();

                if (!people.Any()) return;

                foreach (var person in people)
                {
                    comboBox3.Items.Add(person.Id + ":" + person.Name + " " + person.Surname);
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = comboBox3.SelectedItem.ToString();
            var index = selectedItem.IndexOf(":");
            if (index == -1) return;
            var id = selectedItem.Substring(0, index);

            using (var context = new EdContext())
            {
                var person = context.Owner.SingleOrDefault(a => a.Id.ToString() == id);
                var sensors = context.Sensors.Where(a => a.OwnerId.ToString() == id).ToList();
                if (person == null) return;
                foreach (var sensor in sensors)
                {
                    comboBox4.Items.Add(sensor.Id + ":" + sensor.Place);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            using (var context = new EdContext())
            {
                var maintance = new Maintenance();
                maintance.DataConservation = dateTimePicker1.Value;
                maintance.DataNextConservation = dateTimePicker1.Value.AddDays(30);
                maintance.IdOwner = Int32.Parse(Regex.Match(comboBox3.SelectedItem.ToString(), @"\d+").Value);
                maintance.IdSensor = Int32.Parse(Regex.Match(comboBox4.SelectedItem.ToString(), @"\d+").Value);
                maintance.Work = checkBox1.Checked;
                context.Maintenances.Add(maintance);
                context.SaveChanges();
                SaveLog("Dodaj konserwacje", maintance.IdSensor.ToString(), maintance.IdOwner.ToString());

            }
            GetMaintenance();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > DateTime.Now)
                MessageBox.Show("Nie można wykonać konserwacji dla przyszłej daty");
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = comboBox5.SelectedItem.ToString();
            var index = selectedItem.IndexOf(":");
            if (index == -1) return;
            var id = selectedItem.Substring(0, index);

            using (var context = new EdContext())
            {
                var person = context.Owner.SingleOrDefault(a => a.Id.ToString() == id);
                var sensors = context.Sensors.Where(a => a.OwnerId.ToString() == id).ToList();
                if (person == null) return;
                foreach (var sensor in sensors)
                {
                    comboBox4.Items.Add(sensor.Id + ":" + sensor.Place);
                }
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox7_Enter(object sender, EventArgs e)
        {
            comboBox7.Items.Clear();
            using (var context = new EdContext())
            {
                var people = context.Owner.ToList();

                if (!people.Any()) return;

                foreach (var person in people)
                {
                    comboBox7.Items.Add(person.Id + ":" + person.Name + " " + person.Surname);
                }
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox7.Refresh();
            var selectedItem = comboBox7.SelectedItem.ToString();
            var index = selectedItem.IndexOf(":");
            if (index == -1) return;
            var id = selectedItem.Substring(0, index);

            using (var context = new EdContext())
            {
                var person = context.Owner.SingleOrDefault(a => a.Id.ToString() == id);
                var sensors = context.Sensors.Where(a => a.OwnerId.ToString() == id).ToList();
                if (person == null) return;
                foreach (var sensor in sensors)
                {
                    comboBox8.Items.Add(sensor.Id + ":" + sensor.Place);
                }
            }
            comboBox8.Enabled = true;
        }

        private void comboBox8_Enter(object sender, EventArgs e)
        {
            comboBox8.Items.Clear();
            using (var context = new EdContext())
            {
                var events = context.Events.ToList();

                if (!events.Any()) return;

                foreach (var eventes in events)
                {
                    comboBox8.Items.Add(eventes.Id + " - " + eventes.Date);
                }
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox8.Refresh();
            var selectedItem = comboBox8.SelectedItem.ToString();
            var index = selectedItem.IndexOf(":");
            if (index == -1) return;
            var id = selectedItem.Substring(0, index);
            textBox9.Enabled = true;
            textBox10.Enabled = true;
            textBox11.Enabled = true;
            textBox5.Enabled = true;
            textBox7.Enabled = true;
            textBox8.Enabled = true;
            checkBox2.Enabled = true;
            button4.Enabled = true;
            using (var context = new EdContext())
            {
                var sensors = context.Sensors.SingleOrDefault(a => a.Id.ToString() == id);
                var events = context.Events.Where(a => a.Id.ToString() == id).ToList();
                if (sensors == null) return;
                foreach (var eventes in events)
                {
                    comboBox8.Items.Add(eventes.Id + " - " + eventes.Date);
                }
            }

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_Leave(object sender, EventArgs e)
        {
            DateTime temp;
            if (!DateTime.TryParse(textBox10.Text, out temp))
                MessageBox.Show("Wprowadź poprawną godzinę");
        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_Leave(object sender, EventArgs e)
        {
            DateTime temp;
            //  if(textBox9.Text<textBox10.Text)
            if (!DateTime.TryParse(textBox9.Text, out temp))
                MessageBox.Show("Wprowadź poprawną godzinę");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (var context = new EdContext())
            {
                var interventions = new Intervention();
                var data = DateTime.Parse(Regex.Match(comboBox8.SelectedItem.ToString(), @"\d{2}.\d{2}.\d{4}").Value);
                DateTime dt = DateTime.ParseExact(data.ToShortDateString(), "dd.MM.yyyy", CultureInfo.InvariantCulture);
                interventions.IdOwner = Int32.Parse(Regex.Match(comboBox7.SelectedItem.ToString(), @"\d+").Value);
                interventions.WhenArrive = dt + TimeSpan.Parse(textBox9.Text);
                interventions.IdEvent = Int32.Parse(Regex.Match(comboBox8.SelectedItem.ToString(), @"\d+").Value);
                interventions.WhenLeave = dt + TimeSpan.Parse(textBox10.Text);
                interventions.WhenEnd = dt + TimeSpan.Parse(textBox11.Text);
                interventions.HowLong = interventions.WhenLeave.Hour - interventions.WhenArrive.Hour;
                interventions.DeadVictims = Int32.Parse(textBox5.Text);
                interventions.InjureVictims = Int32.Parse(textBox7.Text);
                interventions.Arson = checkBox2.Checked;
                interventions.Description = textBox8.Text;
                context.Interventions.Add(interventions);
                context.SaveChanges();
                SaveLog("Dodaj interwencje", interventions.IdOwner.ToString(), interventions.Id.ToString());
            }

            GetIntervention();
            ClearTextBoxes(Controls);
            comboBox7.ResetText();
            comboBox8.ResetText();
        }
    


        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox11_Leave(object sender, EventArgs e)
        {
            DateTime temp;
            if (!DateTime.TryParse(textBox11.Text, out temp))
                MessageBox.Show("Wprowadź poprawną godzinę");
        }

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (dataGridView4.Columns[e.ColumnIndex].Name == "refresh")
            {
                dataGridView6.Update();
            }
        }
        
    }
    
}