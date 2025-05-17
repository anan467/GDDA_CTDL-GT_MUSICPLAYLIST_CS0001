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
        private bool isClosing = false;

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
        private DoubleLinkedList playList = new DoubleLinkedList();
        private List<string> originalSongs = new List<string>(); // Lưu danh sách gốc
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
            openFileDialog.Filter = "Audio Files|*.mp3";
            openFileDialog.Title = "Select a song to insert.";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                string title = Path.GetFileNameWithoutExtension(path).Trim();


                string input = Interaction.InputBox(


                    "Enter the position where you want to insert the song(beginning with 0):", "insert song", "0");

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
        private void btPREVIOUS_Click(object sender, EventArgs e)
        {
            if (playList == null || playList.CurrentSong == null)
            {
                MessageBox.Show("There are no songs in the list.", "Notificaiton",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (playList.CurrentSong != null && playList.CurrentSong.PreviousNode != null)
            {
                playList.Previous();
                listBox1.SelectedItem = playList.CurrentSong.SongTitle;

                player.controls.stop(); // Dừng trước
                player.URL = playList.CurrentSong.PathToFile;

                player.controls.play();
                timer1.Start();
                btPLAY.Text = "Pause";
            }
            else
            {
                MessageBox.Show("This is the first song.","Notification", 
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
            
           else if (playList.CurrentSong != null && playList.CurrentSong.NextNode != null)
            {
                playList.Next();
                listBox1.SelectedItem = playList.CurrentSong.SongTitle;

                player.controls.stop(); // Dừng bài hiện tại (rất quan trọng)
                player.URL = playList.CurrentSong.PathToFile;

                // Chờ Media Player load xong URL trước khi play
                player.controls.play();
                timer1.Start();
                btPLAY.Text = "Pause"; // cập nhật nút về trạng thái đang phát
            }
            else
            {
                // Nếu đã ở cuối danh sách
                MessageBox.Show("This is the end of the playlist.", "Notification",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //nút để phát và hát và tạm dừng bài hát
        private void btPLAY_Click(object sender, EventArgs e)
        {
            // Nếu chưa có bài hát nào đang được chọn/phát
            if (playList.CurrentSong == null)
            {
                if (listBox1.SelectedIndex != -1)
                {
                    string selectedSongTitle = listBox1.SelectedItem.ToString();
                    var songNode = playList.Searching(selectedSongTitle);
                    if (songNode != null)
                    {
                        playList.CurrentSong = songNode;
                        PlaySelectedSong(songNode.PathToFile);
                        btPLAY.Text = "Pause";
                    }
                }
                else
                {
                    MessageBox.Show("Please select a song first.", "Notice");
                }
            }
            else
            {
                // Kiểm tra trạng thái hiện tại của MediaPlayer
                var playerState = player.playState;

                if (playerState == WMPPlayState.wmppsPlaying)
                {
                    player.controls.pause();
                    btPLAY.Text = "Play";
                }
                else if (playerState == WMPPlayState.wmppsPaused )
                {
                    player.controls.play();
                    btPLAY.Text = "Pause";
                }
                else
                {
                    player.controls.stop();
                    btPLAY.Text = "▶";
                }
            }
        }

        private void PlayNextSong()// tiếp tục highlight bài hát tiếp theo trong listbox nếu đã phát xong bài trước đó
        {
            if (isClosing || player == null)
                return;

            string nextPath = playList.CurrentSong.NextNode.PathToFile;
            if (!string.IsNullOrEmpty(nextPath))
            {
                player.URL = nextPath;
                player.controls.play();
            }
        }

        private void Player_PlayStateChange(int newState)// cập nhật trạng thái khi bài nhạc phát hết.
        {
         
            if (isClosing || player == null) return;
            if ((WMPPlayState)newState == WMPPlayState.wmppsStopped) // MediaEnded
            {
                timer1.Stop();
                try
                {
                    PlayNextSong(); // Đảm bảo hàm này không gọi player khi null
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi chuyển bài tiếp theo: " + ex.Message);
                }

                this.btPLAY.Click += new EventHandler(this.btPLAY_Click);
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
        //giải phóng dung lượng 
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            isClosing = true; // Báo hiệu đang đóng form
            base.OnFormClosing(e);

            // Giải phóng media player
            if (player != null)
            {
                try
                {
                    player.controls.stop();
                    player.PlayStateChange -= Player_PlayStateChange;//ngắt sự kiện
                    timer1.Tick -= timer1_Tick;
                    trackBar1.MouseUp -= TrackBar1_MouseUp ;
                    trackBar1.MouseDown -= TrackBar1_MouseDown;
                     player.close();
                    player = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi đóng player: " + ex.Message);
                }

                // Dọn dẹp tài nguyên không quản lý
                GC.Collect();
                GC.WaitForPendingFinalizers();
                MessageBox.Show("Completed cleaning!");
            }
        }
        private void btAddAll_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 Files|*.mp3";
            openFileDialog.Title = "Select MP3 Songs";
            openFileDialog.Multiselect = true; // Cho phép chọn nhiều bài hát

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string path in openFileDialog.FileNames)
                {
                    try
                    {
                        string title = Path.GetFileNameWithoutExtension(path).Trim();

                        // Kiểm tra xem bài hát đã tồn tại chưa
                        if (playList.Searching(title) != null)
                            continue; // Bỏ qua bài hát đã tồn tại

                        // Thêm bài hát mới
                        playList.Add(path, title);
                        listBox1.Items.Add(title);
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi tại bài: " + path + "\nChi tiết: " + ex.Message);
                    }
                }

                MessageBox.Show("Songs added successfully.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            if (player != null)
            {
                player.controls.stop();
                player.close(); // Tùy chọn: giải phóng media
            }
            timer1.Stop();

            // Xoá dữ liệu danh sách
            playList.Clear();                  // Xoá danh sách liên kết
            listBox1.Items.Clear();            // Xoá trên giao diện
            playList.CurrentSong = null;

            // Reset trackbar và label thời gian
            trackBar1.Value = 0;
            trackBar1.Maximum = 100; // hoặc 0, tuỳ cách bạn setup ban đầu
            lbCURRENTTIME.Text = "00:00";
            lblTOTALTIME.Text = "00:00";

            MessageBox.Show("Playlist cleared.");
        }
    }
    //lớp tạo nút cho danh sách liên kết
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
        public override string ToString()// để hiện tên bài hát 
        {
            return SongTitle;
        }
    }

    // lớp tạo danh sáhc liên kêt cho danh sách bài hát
    internal class DoubleLinkedList
    {
        public NodeSong FirstSong { get; private set; } = null;// nút đầu tiên trong danh sách phát 
        public NodeSong LastSong { get; private set; } = null;// nút cuối cùng trong danh sách phát
        public NodeSong CurrentSong { get; set; }// nút chỉ bài hát đang phất
        public int SongCount { get; private set; }// số lượng bài hát có trong danh sách 
                                                  //thêm bài hát vào danh sách phát
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
        //dùng cho removeAt để bỏ bài nhạc mà không cần phải đếm số thứ tự bài hát để loại bỏ
        public NodeSong Searching(string title)
        {
            string lowertitle = title.ToLower(); // chuẩn hóa từ khóa về chữ thường
            NodeSong current = FirstSong;
            while (current != null)// phải kiểm tra xem current có đang null hay không không thì sx gặp lỗi Object reference not set to an instance of an object.
            {
                if (current.SongTitle.ToLower() == lowertitle)
                {
                    return current;
                }
                current = current.NextNode;
            }
            return null;
        }
        //xóa bài hát khỏi danh sách phát 
        public void RemoveAt(string title)
        {
            NodeSong current = Searching(title);
            if (current == null) return;

            if (current.NextNode != null)
                current.NextNode.PreviousNode = current.PreviousNode;
            if (current.PreviousNode != null)
                current.PreviousNode.NextNode = current.NextNode;
            if (current == FirstSong)
                FirstSong = current.NextNode;//cập nhật lại danh sách sau khi bài đâu danh sách bị xóa đi 
           else if (current == LastSong)
                LastSong = current.PreviousNode;//cập nhật lại danh sách sau khi bài cuối danh sách bị xóa đi 

            SongCount--;
        }

        // Tìm node tại chỉ số index dùng cho insert
        public NodeSong Find(int index)
        {
            if (index < 0 || index >= SongCount) return null;

            NodeSong current = FirstSong;
            int i = 0;// i bắt đầu là 0
            while (current != null)
            {
                if (i == index) return current;
                current = current.NextNode;
                i++;
            }
            return null;
        }
        // Chèn vào vị trí bất kỳ mà không cần nhập tên bài hát chỉ cần nhập vị trí bài nhạc 
        public void Insert(int index, string path, string title)
        {
            NodeSong newNode = new NodeSong(path, title);
            if (index == 0)
            {
                newNode.NextNode = FirstSong;
                FirstSong = newNode;
            }
            else if (FirstSong == null )
            {
                FirstSong = LastSong = newNode;
            }
            else
            {
                /* nếu viết là :
           NdoeSong  prev = Find (index);
                newNode.NextNode = prev.NextNode;
                prev.NextNode = newNode;
                newNode.PreviousNode = prev;
                sẽ dễ bị gặp lỗi NullReferenceException khi lỡ Previous.next ban đầu trỏ tới null 
                -> xử lý bằng cách sử dụng next làm trung gian và tạo điều kiện cho nó khi nó null 
              */
                NodeSong prev = Find(index-1);// tìm vị trí bài nhạc đứng trước để chèn vào 
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
        //xóa hết danh sách 
        public void Clear()
        {
            NodeSong current = FirstSong;
            while (SongCount > 0)
            {
                RemoveAt(current.SongTitle);
                current = current.NextNode;
                SongCount--;
            }
        }
        //nút chuyển tiếp nhạc
        public void Next()
        {
            if (CurrentSong != null && CurrentSong.NextNode != null)
            {
                CurrentSong = CurrentSong.NextNode;
            }
            else
                CurrentSong = null;// đảm bảo không lặp mãi mãi một vòng lặp trong PlayNextSong
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
}
