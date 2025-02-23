using System.Text.Json;

namespace Client
{
    public partial class CreateRoom : Form
    {
        private string? SelectedCategory;

        public int RoomID { get; private set; }

        public CreateRoom()
        {

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
            ComboBox CategoriesMenu = CategoriesDropDown;
            CategoriesMenu.Items.AddRange(categories.ToArray());

            if (categories.Count > 0)
            {
                CategoriesMenu.SelectedItem = categories[0];
                SelectedCategory = categories[0];
            }

            CategoriesMenu.SelectedIndexChanged += (sender, e) =>
            {
                SelectedCategory = CategoriesMenu.SelectedItem?.ToString();
            };
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

                RoomID = roomID;
                DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Error creating room", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {

            this.DialogResult = DialogResult.Cancel;
        }


    }
}
