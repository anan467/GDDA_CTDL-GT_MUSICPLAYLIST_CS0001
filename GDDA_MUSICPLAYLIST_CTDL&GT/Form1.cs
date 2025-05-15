using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.VisualBasic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace GDDA_MUSICPLAYLIST_CTDL_GT
{
    public partial class Form1 : Form
    {
        private WMPLib.WindowsMediaPlayer player;
        bool isDragging = false;
        public Form1()
        { 
            //thêm các sự kiện có trong giao diện
            InitializeComponent();
            player = new WMPLib.WindowsMediaPlayer();
           // người dùng có thể lựa chọn đoạn nhạc muốn nghe trên trackbar 
            trackBar1.MouseDown += TrackBar1_MouseDown;
            trackBar1.MouseUp += TrackBar1_MouseUp;
            timer1.Tick += timer1_Tick;
            //kiểm tra trạng thái phát của bài nhạc 
            player.PlayStateChange += Player_PlayStateChange;
        }
        internal class NodeSong
        {
            public string PathToFile { get; set; }// đường dẫn nhạc 
            public string SongTitle { get; set; }// tên bài hát
            public NodeSong PreviousNode { get; set; }// bài hát trước đó
            public NodeSong NextNode { get; set; }// bài hát tiếp theo

            public NodeSong(string pathToFile, string songTitle)
            {
                PathToFile = pathToFile;
                SongTitle = songTitle;
                PreviousNode = null;
                NextNode = null;
            }

            public override string ToString()
            {
                return SongTitle;
            }
        }

        internal class DoubleLinkedList
        {
            public NodeSong FirstSong { get; private set; } = null;// nút đầu tiên trong danh sách phát 
            public NodeSong LastSong { get; private set; } = null;// nút cuối cùng trong danh sách phát
            public NodeSong CurrentSong { get; set; }// nút chỉ bài hát đang phất
            public int SongCount { get; private set; }// số lượng bài hát có trong danh sách 

            public void Add(string path, string title)
            {
                NodeSong newNode = new NodeSong(path, title);
                if (FirstSong == null)
                {
                    FirstSong = LastSong = newNode;
                }
                else
                {
                    FirstSong.NextNode = newNode;
                    newNode.PreviousNode = FirstSong;
                    newNode.NextNode = LastSong;
                    LastSong.PreviousNode  = newNode;
                }
                SongCount++;
            }
            //dùng cho removeAt để bỏ bài nhạc mà không cần phải đếm số thứ tự bài hát để loại bỏ
            public NodeSong Searching(string keyword)
            {
                string lowerKeyword = keyword.ToLower(); // chuẩn hóa từ khóa về chữ thường
                NodeSong current = FirstSong;

                while (current != null)
                {
                    if (current.SongTitle.ToLower().Contains(lowerKeyword))
                    {
                        return current;
                    }
                    current = current.NextNode;
                }

                return null;
            }
            //xóa bài hát khỏi danh sách phát 
            public void RemoveAt( string title)
            {
                NodeSong current = Searching(title);
                if (current == null) return;

                if (current.NextNode != null)
                    current.NextNode.PreviousNode = current.PreviousNode;
                if (current.PreviousNode != null)
                    current.PreviousNode.NextNode = current.NextNode;

                if (current == FirstSong)
                    FirstSong = current.NextNode;//đầu danh sách là bài hát sau bài đã bị xóa
                if (current == LastSong)
                    LastSong = current.PreviousNode;

                SongCount--;
            }
            // Tìm node tại chỉ số index dùng cho insert
            public NodeSong Find(int index)
            {
                if (index < 0 || index >= SongCount) return null;

                NodeSong current = FirstSong;
                int i = 0;
                while (current != null)
                {
                    if (i == index) return current;
                    current = current.NextNode;
                    i++;
                }
                return null;
            }

            // Chèn vào vị trí bất kỳ
            public void Insert(int index, string path, string title)
            {
                NodeSong newNode = new NodeSong(path, title);

                if (index <= 0 || FirstSong == null)
                {
                    newNode.NextNode = FirstSong;
                    if (FirstSong != null)
                        FirstSong.PreviousNode = newNode;
                    FirstSong = newNode;
                    if (LastSong == null) LastSong = newNode;
                }
                else
                {
                    NodeSong prev = Find(index - 1);
                    if (prev == null)
                    {
                        Add(path, title);
                        return;
                    }

                    NodeSong next = prev.NextNode;
                    prev.NextNode = newNode;
                    newNode.PreviousNode = prev;
                    newNode.NextNode = next;
                    if (next != null)
                        next.PreviousNode = newNode;
                    else
                        LastSong = newNode;
                }
                SongCount++;
            }

            // Hoán đổi 2 node dựa theo vị trí
            public void Swap(int index1, int index2)
            {
                if (index1 == index2) return;

                NodeSong node1 = Find(index1);
                NodeSong node2 = Find(index2);
                if (node1 == null || node2 == null) return;

                // Hoán đổi dữ liệu
                string tempPath = node1.PathToFile;
                string tempTitle = node1.SongTitle;

                node1.PathToFile = node2.PathToFile;
                node1.SongTitle = node2.SongTitle;

                node2.PathToFile = tempPath;
                node2.SongTitle = tempTitle;
            }

            // Sắp xếp theo tên bài hát-vẫn chưa hoàn thiện
            public void Sort()
            {
                if (SongCount < 2) return;

                bool swapped;
                do
                {
                    swapped = false;
                    NodeSong current = FirstSong;
                    while (current != null && current.NextNode != null)
                    {
                        if (string.Compare(current.SongTitle, current.NextNode.SongTitle, StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            // Hoán đổi dữ liệu
                            string tempPath = current.PathToFile;// các biến trung gian 
                            string tempTitle = current.SongTitle;

                            current.PathToFile = current.NextNode.PathToFile;
                            current.SongTitle = current.NextNode.SongTitle;

                            current.NextNode.PathToFile = tempPath;
                            current.NextNode.SongTitle = tempTitle;

                            swapped = true;
                        }
                        current = current.NextNode;
                    }
                } while (swapped);
            }
            //nút chuyển tiếp nhạc
            public void Next()
            {
                if (CurrentSong != null && CurrentSong.NextNode != null)
                {
                    CurrentSong = CurrentSong.NextNode;
                }
            }
            //nút lùi nhạc
            public void Previous()
            {
                if (CurrentSong != null && CurrentSong.PreviousNode != null)
                {
                    CurrentSong = CurrentSong.PreviousNode;
                }
            }
            public List<NodeSong> GetAllNodesong()
            {
                List<NodeSong> list = new List<NodeSong>();
                NodeSong current = FirstSong;
                while (current != null)
                {
                    list.Add(current);
                    current = current.NextNode;
                }
                return list;
            }
        }
        private DoubleLinkedList playList = new DoubleLinkedList();
        private void btADD_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();// để mở ra File Explorer
            openFileDialog.Filter = "Audio Files|*.mp3;*.wav;*.wma";// lọc các đạng file nhạc phù hợp.
            openFileDialog.Title = "Select a Song File";

            if (openFileDialog.ShowDialog() == DialogResult.OK) // hiển thị ra hộp thọai, nếu bấm Open thì điều kiện được kích hoạt.
            {
                string path = openFileDialog.FileName;
                string title = Path.GetFileNameWithoutExtension(path).Trim();// Trả về tên file từ một đường dẫn đầy đủ, nhưng loại bỏ phần mở rộng (ví dụ: .txt, .jpg, .pdf,...).
                                                                             // đồn thời loại bỏ các khoảng trắng ở đầu và cuối chuỗi đường dẫn 
               //kiểm tra bài mới thêm vào có trùng tên với bài đã có trong danh sách hay không?
                bool exists = listBox1.Items.Cast<string>() //ép tất cả các phần tử trong danh sách về kiểu string.
                                .Any(item => item.Equals(title, StringComparison.OrdinalIgnoreCase));//trong thư viện linq dùng để kiểm tra xem có ít nhất mọt phần tử thỏa mãn điều kiện không?
                                                                                                     //StringComparison.OrdinalIgnoreCase là tùy chọn so sánh. So sánh chính xác byte(ordinal), nhưng không phân biệt chữ hoa/ chữ thường.

                if (exists)
                {
                    DialogResult result = MessageBox.Show(
                "This song already exists in the list. Do you still want to add it again?",
                "Notification",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                        return; // Người dùng chọn không thêm → thoát
                }

                // Thêm bài hát
                playList.Add(path, title);
                playList.CurrentSong = playList.LastSong;
                listBox1.Items.Add(title);

            }
        }
        private List<string> originalSongs = new List<string>(); // Lưu danh sách gốc
        private bool isFiltered = false; // Kiểm tra trạng thái lọc
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string selectedSong = listBox1.SelectedItem.ToString();

                // Nếu đang ở chế độ lọc, hiển thị lại toàn bộ danh sách
                if (isFiltered)
                {
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(originalSongs.ToArray());
                    isFiltered = false;

                    // Giữ nguyên bài hát đã chọn
                    listBox1.SelectedItem = selectedSong;
                }

                // Cập nhật bài hát hiện tại và phát nhạc
                var node = playList.Searching(selectedSong);
                if (node != null)
                {
                    playList.CurrentSong = node;
                    // Thêm code phát nhạc ở đây nếu cần
                }
            }
        }
        private void UpdateMusicListBox()
        {
            listBox1.Items.Clear(); // Xóa danh sách hiện tại trong ListBox

            List<NodeSong> allSongs = playList.GetAllNodesong(); // Lấy toàn bộ bài hát từ danh sách liên kết

            foreach (NodeSong song in allSongs)
            {
                listBox1.Items.Add(song.SongTitle); // Thêm từng tiêu đề bài hát vào ListBox
            }

            // Cập nhật danh sách gốc nếu có lọc tìm kiếm
            originalSongs = allSongs.Select(s => s.SongTitle).ToList();

            if (allSongs.Count == 0)
{
    originalSongs.Clear();
}


        }
        private void PlaySelectedSong(string filePath)//nút phát nhạc đang được chọn, tức là phải chọn bài hát có trong listbox trước rồi mới bấm nút phát để phát nhạc
        {
            try
            {
                // Dừng phát nếu đang phát
                player.controls.stop();

                // Thiết lập file nhạc mới
                player.URL = filePath;

                // Bắt đầu phát
                player.controls.play();
                timer1.Interval = 1000; // cập nhật mỗi giây
                timer1.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR WHILE PLAYING SONGS" + ex.Message, "ERROR",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PlayNextSong()// tiếp tục highlight bài hát tiếp theo trong listbox nếu đã phát xong bài trước đó
        {
            if (playList == null || playList.CurrentSong == null)
                return;

            playList.Next(); // Chuyển sang bài tiếp theo

            if (playList.CurrentSong != null)
            {
                listBox1.SelectedItem = playList.CurrentSong.SongTitle; // Cập nhật ListBox
                PlaySelectedSong(playList.CurrentSong.PathToFile); // Phát nhạc
                this.Text = "Currently playing: " + playList.CurrentSong.SongTitle; // (Tùy chọn) cập nhật tiêu đề
            }
            else
            {
                timer1.Stop();
                MessageBox.Show("End song.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Player_PlayStateChange(int newState)// cập nhật trạng thái khi bài nhạc phát hết.
        {
            if (newState == 8) // MediaEnded
            {
                timer1.Stop();
                // Gọi hàm PlayNextSong() nếu bạn có danh sách nhạc
                PlayNextSong();
            }
        }

        private void btREMOVE_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem người dùng đã chọn bài nào trong ListBox chưa
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Please choose song that you want to be removed.",
                                "Notification.",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }

            // Lấy tên bài hát được chọn
            string selectedSong = listBox1.SelectedItem.ToString();

            // Hiển thị hộp thoại xác nhận xóa
            DialogResult result = MessageBox.Show(
                    $"Are you sure you want to remove '{selectedSong}'?",
                    "confirm deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Gọi phương thức RemoveAt với tên bài (ở đây dùng cùng tên cho cả link và tenbh theo code mẫu)

                playList.RemoveAt(selectedSong);
                UpdateMusicListBox();

            }
        }

        private void btINSERT_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio Files|*.mp3;*.wav;*.wma";
            openFileDialog.Title = "Select a song to insert.";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                string title = Path.GetFileNameWithoutExtension(path).Trim();


                string input = Interaction.InputBox(


                    "Enter the position where you want to insert the song (0 - beginning of the list):", "insert song", "0");

                if (int.TryParse(input, out int index))
                {
                    playList.Insert(index, path, title);
                    UpdateMusicListBox();
                }
                else
                {
                    MessageBox.Show("Please enter a valid number.");
                }
            }
        }

        private void btSWAP_Click(object sender, EventArgs e)
        {

            string input1 = Interaction.InputBox(

        "Enter the first position:", "Swap", "0");
            string input2 = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the second position:", "Swap", "1");

            if (int.TryParse(input1, out int index1) && int.TryParse(input2, out int index2))
            {
                playList.Swap(index1, index2);
                UpdateMusicListBox();
            }
            else
            {
                MessageBox.Show("Please enter a valid number.");
            }
        }

        private void btSORT_Click(object sender, EventArgs e)
        {
            playList.Sort();
            UpdateMusicListBox();
            MessageBox.Show("The song list has been arranged in order of song title.");
        }

        private void btPREVIOUS_Click(object sender, EventArgs e)
        {
            if (playList == null || playList.CurrentSong == null)
            {
                MessageBox.Show("There are no songs in the list.", "Notificaiton",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            playList.Previous();
            if (playList.CurrentSong != null)
            {

                PlaySelectedSong(playList.CurrentSong.PathToFile);


                MessageBox.Show("Currently playing: " + playList.CurrentSong.SongTitle,
                     "Notification",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information);

            }
            else
            {
                // Nếu đã ở cuối danh sách
                MessageBox.Show("This is the end of the playlist", "Notification",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btNEXT_Click(object sender, EventArgs e)
        {
            if (playList == null || playList.CurrentSong == null)
            {
                MessageBox.Show("There is not any songs in the playlist", "Notification",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            playList.Next();

            if (playList.CurrentSong != null)
            {
                MessageBox.Show("Currently playing: " + playList.CurrentSong.SongTitle,
                     "Notification",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information);
                listBox1.SelectedItem = playList.CurrentSong.SongTitle;
            }
            else
            {
                // Nếu đã ở cuối danh sách
                MessageBox.Show("This is the end of the playlist.", "Notification",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btPLAY_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a song from the playlist.", "Notification",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string selectedSongTitle = listBox1.SelectedItem.ToString();

            // Tìm bài hát trong danh sách
            var songNode = playList.Searching(selectedSongTitle);

            if (songNode != null)
            {
                // Cập nhật bài hát hiện tại
                playList.CurrentSong = songNode;

                // Phát bài hát
                PlaySelectedSong(songNode.PathToFile);

                // Cập nhật tiêu đề form (tùy chọn)
                this.Text = "Currently Playing: " + songNode.SongTitle;
            }
            else
            {
                MessageBox.Show("Could not find the selected song.", "ERROR",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (player.currentMedia != null && !isDragging)
            {
                double duration = player.currentMedia.duration;
                double current = player.controls.currentPosition;

                if (duration > 0)
                {
                    trackBar1.Maximum = (int)duration;
                    trackBar1.Value = Math.Min((int)current, trackBar1.Maximum);
                }
                lbCURRENTTIME.Text = TimeSpan.FromSeconds(current).ToString(@"mm\:ss");
                lblTOTALTIME.Text = TimeSpan.FromSeconds(duration).ToString(@"mm\:ss");
            }
        }
        private void TrackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
        }

        private void TrackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            if (player.currentMedia != null)
            {
                player.controls.currentPosition = trackBar1.Value;
            }
            isDragging = false;
        }
    }
}
