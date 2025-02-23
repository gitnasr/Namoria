using System.Text.Json;

namespace Client
{
    public partial class CreateRoom : Form
    {
        private string? SelectedCategory;
        private Form parentForm;


        public CreateRoom(Form ParentForm)
        {
            parentForm = ParentForm;

            InitializeComponent();
        }

        private async Task GetAllCategories()
        {
            try
            {


                Connection.SendToServer(PlayEvents.GET_CATEGORIES);
                string response = await Task.Run(() => Connection.ReadFromServer.ReadString());
                ProcessedEvent eventResult = EventProcessor.ProcessEvent(response);
                if (eventResult.Event == PlayEvents.SEND_CATEGORIES)
                {
                    string ReceivedCategoriesAsString = eventResult.Data;
                    List<string>? CategoriesAsList = JsonSerializer.Deserialize<List<string>>(ReceivedCategoriesAsString);
                    if (CategoriesAsList != null && CategoriesAsList.Count > 0)
                    {
                        this.Invoke(new Action(() =>
                        {
                            DisplayCategories(CategoriesAsList);
                        }));
                    }


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending message: " + ex.Message);
            }
        }

        private void DisplayCategories(List<string> categories)
        {
            int i = 0;
            foreach (var category in categories)
            {
                RadioButton radioButton = new RadioButton();
                radioButton.Text = category;
                radioButton.Click += RadioButton_Click;
                radioButton.AutoSize = true;
                CategoriesTable.SetColumn(radioButton, i);
                CategoriesTable.Controls.Add(radioButton);

                i++;

            }

        }

        private void RadioButton_Click(object? sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender!;
            SelectedCategory = radioButton.Text;
        }




        private async void CreateRoom_Load(object sender, EventArgs e)
        {
            await GetAllCategories();
        }


        private async void button1_Click_1Async(object sender, EventArgs e)
        {
            if (SelectedCategory == null)
            {
                MessageBox.Show("How we will start a game for you without a chosen category ?", "Are you serious ?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Connection.SendToServer(PlayEvents.CREATE_ROOM, SelectedCategory);

            string response = await Task.Run(() => Connection.ReadFromServer.ReadString());
            ProcessedEvent processed = EventProcessor.ProcessEvent(response);
            if (processed.Event == PlayEvents.ROOM_CREATED)
            {
                bool isParsed = int.TryParse(processed.Data, out int roomID);
                if (!isParsed)
                {
                    MessageBox.Show("Error creating room", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Form GameForm = new Game(roomID);

                GameForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Error creating room", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.parentForm.Show();
            this.Close();
        }

        private void CreateRoom_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.parentForm != null)
            {

                parentForm.Show();

            }
            else
            {
                Application.Exit();
            }

        }
    }
}
