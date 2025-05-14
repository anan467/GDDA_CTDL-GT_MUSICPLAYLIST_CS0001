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

namespace GDDA_MUSICPLAYLIST_CTDL_GT
{
    public partial class Form1 : Form
    {
        private WMPLib.WindowsMediaPlayer player;
        bool isDragging = false;
        public Form1()
        { 
            InitializeComponent();
            player = new WMPLib.WindowsMediaPlayer();
            trackBar1.MouseDown += TrackBar1_MouseDown;
            trackBar1.MouseUp += TrackBar1_MouseUp;
            timer1.Tick += timer1_Tick;
            player.PlayStateChange += Player_PlayStateChange;
        }
        internal class NodeSong
        {
            public string PathToFile { get; set; }
            public string SongTitle { get; set; }
            public NodeSong PreviousNode { get; set; }
            public NodeSong NextNode { get; set; }

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
            public NodeSong FirstSong { get; private set; } = null;
            public NodeSong LastSong { get; private set; } = null;
            public NodeSong CurrentSong { get; set; }
            public int SongCount { get; private set; }

            public void Add(string path, string title)
            {
                NodeSong newNode = new NodeSong(path, title);
                if (FirstSong == null)
                {
                    FirstSong = LastSong = newNode;
                }
                else
                {
                    LastSong.NextNode = newNode;
                    newNode.PreviousNode = LastSong;
                    LastSong = newNode;
                }
                SongCount++;
            }

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


            public void RemoveAt( string title)
            {
                NodeSong current = Searching(title);
                if (current == null) return;

                if (current.NextNode != null)
                    current.NextNode.PreviousNode = current.PreviousNode;
                if (current.PreviousNode != null)
                    current.PreviousNode.NextNode = current.NextNode;

                if (current == FirstSong)
                    FirstSong = current.NextNode;
                if (current == LastSong)
                    LastSong = current.PreviousNode;

                SongCount--;
            }
            // Tìm node tại chỉ số index
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

            // Sắp xếp theo tên bài hát
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
                            string tempPath = current.PathToFile;
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio Files|*.mp3;*.wav;*.wma";
            openFileDialog.Title = "Select a Song File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                string title = Path.GetFileNameWithoutExtension(path).Trim();

                bool exists = listBox1.Items.Cast<string>()
                                .Any(item => item.Equals(title, StringComparison.OrdinalIgnoreCase));


                if (exists)
                {
                    DialogResult result = MessageBox.Show(
                "Bài hát này đã tồn tại trong danh sách. Bạn vẫn muốn thêm lại?",
                "Thông báo",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                        return; // Người dùng chọn không thêm → thoát
                }

                // Thêm bài hát
                playList.Add(path, title);
                playList.CurrentSong = playList.LastSong;
                UpdateMusicListBox();
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
                MessageBox.Show("Lỗi khi phát bài hát: " + ex.Message, "Lỗi",
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
                this.Text = "Đang phát: " + playList.CurrentSong.SongTitle; // (Tùy chọn) cập nhật tiêu đề
            }
            else
            {
                timer1.Stop();
                MessageBox.Show("Đã phát hết danh sách bài hát.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Vui lòng chọn bài hát cần xóa khỏi danh sách.",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }

            // Lấy tên bài hát được chọn
            string selectedSong = listBox1.SelectedItem.ToString();

            // Hiển thị hộp thoại xác nhận xóa
            DialogResult result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa bài hát '{selectedSong}' không?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Gọi phương thức RemoveAt với tên bài (ở đây dùng cùng tên cho cả link và tenbh theo code mẫu)
                playList.RemoveAt(selectedSong);

                // Cập nhật lại danh sách trong ListBox
                UpdateMusicListBox();

            }
        }

        private void btINSERT_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio Files|*.mp3;*.wav;*.wma";
            openFileDialog.Title = "Chọn bài hát để chèn";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                string title = Path.GetFileNameWithoutExtension(path).Trim();

                string input = Interaction.InputBox(
                    "Nhập vị trí muốn chèn bài hát (0 - đầu danh sách):", "Chèn bài hát", "0");

                if (int.TryParse(input, out int index))
                {
                    playList.Insert(index, path, title);
                    UpdateMusicListBox();
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập số hợp lệ.");
                }
            }
        }

        private void btSWAP_Click(object sender, EventArgs e)
        {
            string input1 = Interaction.InputBox(
        "Nhập vị trí thứ nhất:", "Hoán đổi", "0");
            string input2 = Microsoft.VisualBasic.Interaction.InputBox(
                "Nhập vị trí thứ hai:", "Hoán đổi", "1");

            if (int.TryParse(input1, out int index1) && int.TryParse(input2, out int index2))
            {
                playList.Swap(index1, index2);
                UpdateMusicListBox();
            }
            else
            {
                MessageBox.Show("Vui lòng nhập chỉ số hợp lệ.");
            }
        }

        private void btSORT_Click(object sender, EventArgs e)
        {
            playList.Sort();
            UpdateMusicListBox();
            MessageBox.Show("Đã sắp xếp danh sách bài hát theo thứ tự tên bài.");
        }

        private void btPREVIOUS_Click(object sender, EventArgs e)
        {
            if (playList == null || playList.CurrentSong == null)
            {
                MessageBox.Show("Không có bài hát nào trong danh sách", "Thông báo",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            playList.Previous();
            if (playList.CurrentSong != null)
            {
                
                listBox1.SelectedItem = playList.CurrentSong.SongTitle;
                PlaySelectedSong(playList.CurrentSong.PathToFile);
                MessageBox.Show("Đang phát: " + playList.CurrentSong.SongTitle,
                     "Thông báo",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information);
            }
            else
            {
                // Nếu đã ở cuối danh sách
                MessageBox.Show("Đã đến cuối danh sách", "Thông báo",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btNEXT_Click(object sender, EventArgs e)
        {
            if (playList == null || playList.CurrentSong == null)
            {
                MessageBox.Show("Không có bài hát nào trong danh sách", "Thông báo",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            playList.Next();

            if (playList.CurrentSong != null)
            {
                MessageBox.Show("Đang phát: " + playList.CurrentSong.SongTitle,
                     "Thông báo",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information);
                listBox1.SelectedItem = playList.CurrentSong.SongTitle;
            }
            else
            {
                // Nếu đã ở cuối danh sách
                MessageBox.Show("Đã đến cuối danh sách", "Thông báo",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btPLAY_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn bài hát từ danh sách", "Thông báo",
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
                this.Text = "Đang phát: " + songNode.SongTitle;
            }
            else
            {
                MessageBox.Show("Không tìm thấy bài hát được chọn", "Lỗi",
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
