using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using System.Windows.Threading;

namespace Hit_and_Blow
{
    public partial class MainPage : PhoneApplicationPage
    {
        private string Question;
        private List<int> Numeric_list = new List<int>();
        private List<string> Str_Msg_list = new List<string>();
        private int hit_number, blow_number,level_no,try_no,max_time,hint_count;
        private bool hint_sw;

        // コンストラクター
        public MainPage()
        {
            InitializeComponent();

            //テーマを取得
            //Visibility light = (Visibility)Resources["PhoneLightThemeVisibility"];
            //Visibility dark = (Visibility)Resources["PhoneDarkThemeVisibility"];
            //if (light == System.Windows.Visibility.Visible)
            //{
            //    // テーマはLight！！
            //    this.set_light_tema();
            //}
            //if (dark == System.Windows.Visibility.Visible)
            //{
            //    // テーマはDark！！
            //    this.set_dark_tema();
            //}

            //初期化
            this.level_no = 1;

            this.init(this.level_no);
           
            //タイマー初期化
            DispatcherTimer tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromSeconds(1); //間隔は１秒おき
            tmr.Tick += new EventHandler(tmr_Tick); //tmr_Tickイベントハンドラを追加
            tmr.Start();
        }
        //Lightテーマの設定
        //private void set_light_tema()
        //{
        //   // this.Txt_Ans.Foreground = new SolidColorBrush(Colors.Black);
        //   // this.Txt_Hint_MSG.Foreground = new SolidColorBrush(Colors.Black);
        //   // this.ContentPanel.Background = new SolidColorBrush(Colors.White);
        //   // this.Lbl_DSP.Foreground = new SolidColorBrush(Colors.White);
        //   // this.Lst_MSG.Foreground = new SolidColorBrush(Colors.Black);
        //   // this.toggleSwitch.Background = new SolidColorBrush(Colors.Black);
        //   // this.Btn_Check.BorderBrush = new SolidColorBrush(Colors.Black);

        //}
        ////Darkテーマの設定
        //private void set_dark_tema()
        //{
        //  //  this.Txt_Ans.Foreground = new SolidColorBrush(Colors.Black);
        //  //  this.Txt_Hint_MSG.Foreground = new SolidColorBrush(Colors.White);
        //  //  this.ContentPanel.Background = new SolidColorBrush(Colors.Black);
        //  //  this.Lbl_DSP.Foreground = new SolidColorBrush(Colors.White);
        //  //  this.Lst_MSG.Foreground = new SolidColorBrush(Colors.White);
        //  //  this.toggleSwitch.Background = new SolidColorBrush(Colors.White);
        //  //  this.Btn_Check.BorderBrush = new SolidColorBrush(Colors.White);
        //}
        //tmr_Tickイベント
        private void tmr_Tick(object sender, EventArgs e)
        {
            //プログレスバーを下げていく
            this.Bar.Value -= (100.0/this.max_time);
            if (this.Bar.Value == 0.0) { 
                //タイムアップ
                MessageBoxResult r = MessageBox.Show("Time UP！！", "", MessageBoxButton.OK);
                if (r == MessageBoxResult.OK)
                {
                    //最初から
                    this.level_no = 1;
                    this.init(this.level_no);
                }
            }
        }
        //ゲームを初期化します。
        //引数：解読する桁数。３以上９以下
        private void init(int in_level_no)
        {
            //リスト初期化
            this.Numeric_list.Clear();
            for (int i = 0; i < 9; ++i)
            {
                this.Numeric_list.Add(i + 1);
            }
            //挑戦する桁数セット
            this.try_no = in_level_no + 2;//3桁からスタート
            this.Lbl_DSP.Text = question_no(this.try_no);    //????を表示
            //回答欄クリア
            this.Txt_Ans.Text = string.Empty;
            this.Question = string.Empty;
            this.hint_count = level_no;
            this.Txt_Hint_MSG.Text = "ヒントの使用回数はあと" + this.hint_count + "回です";
            this.hit_number = 0;
            this.blow_number = 0;
            this.Lst_MSG.Items.Clear();
            this.Str_Msg_list.Clear();

            //レベル表示
            this.Txt_Level.Text = "LV:" + in_level_no;
            //残り時間
            this.max_time = 80 - (this.level_no * 10);
            //プログレスバーセット
            this.Bar.Value = 100.0;
            //回答の最大文字長セット
            this.Txt_Ans.MaxLength = this.try_no;
            //ヒントスイッチ表示
            this.toggleSwitch.Visibility = System.Windows.Visibility.Visible;
            this.hint_sw = false;
            //問題生成
            this.Question = this.make_question(this.try_no);
          //  textBox2.Text = question; //デバック用

        }
        //チャレンジする桁数分"？"を表示します
        private string question_no(int in_try_number)
        {
            string str_tmp = string.Empty;
            for (int i = 0; i < in_try_number; ++i)
            {
                str_tmp += "?";
            }
            return str_tmp;
         }
        //１～９の格納されたリストからランダムに一つ数字を取り出します。
        //０が出ると何かおかしい
        private int get_number()
        {
            int idx;
            int number;    
            int seed = Environment.TickCount;          
            Random rnd = new System.Random(seed++);     //乱数生成
            idx = rnd.Next(0,this.Numeric_list.Count);  //1から要素数分の乱数生成
            try
            {
                number = this.Numeric_list[idx];
                this.Numeric_list.RemoveAt(idx);
            }
            catch (System.ArgumentException)
            {           
                number = 0;
            }
            return number;
        }
        //問題を作成し、文字列として返します。
        private string make_question(int in_try_number)
        {
            string str_tmp = string.Empty;
            for (int i = 0; i < in_try_number; ++i){
             str_tmp += this.get_number().ToString();
            }
            return str_tmp;
        }
        //解答チェック
        private void chk_answer(int in_try_number,string in_question,string in_answer,bool hint_sw)
        {
            //そもそも桁数がちがう
            if (in_answer.Length != in_try_number)
            {
                return;
            }
            string chk_number,chk_number2;
            this.hit_number=0;
            this.blow_number=0;
            for (int i = 0; i < in_try_number; ++i)
            {
                for (int j = 0; j < in_try_number; ++j)
                {
                    chk_number = in_question.Substring(i, 1);
                    chk_number2 = in_answer.Substring(j,1);
                    if (chk_number == chk_number2)
                    {
                        if (i == j)
                        {
                            //取り出した位置（桁）が同じ
                            this.hit_number += 1;
                            //ヒントモード
                            if (hint_sw)
                            {
                                this.hint_count -= 1;
                                if (this.hint_count < 0) { this.hint_count = 0; }
                                this.Lbl_DSP.Text = this.set_number(this.Lbl_DSP.Text, i, chk_number);
                                if (this.hint_count <= 0)
                                {
                                    //非表示
                                    this.toggleSwitch.Visibility = System.Windows.Visibility.Collapsed;
                                    this.hint_sw = false;
                                }
                            }
                        }
                        else
                        {
                            //位置が違う
                            this.blow_number += 1;
                        }
                        this.Bar.Value = 100.0;    //バーを回復させる
                    }
                  }
                }
            //メッセージを記録
             this.Str_Msg_list.Add("Input Number:" + this.Txt_Ans.Text + " " + this.hit_number + "Hit " + this.blow_number + "Blow");
             this.Lst_MSG.Items.Clear();
             this.Str_Msg_list.Reverse();
             foreach (string item in this.Str_Msg_list)
             {
                 this.Lst_MSG.Items.Add(item);
             }
             this.Str_Msg_list.Reverse();
            }
        //文字列の指定した位置にある１文字を置換した文字列を返す
        private string set_number( string in_string,int in_offset,string in_set_char) {
            string str_tmp = string.Empty;
            if (in_offset > 0)
            {
                str_tmp += in_string.Substring(0, in_offset);
            }
            str_tmp += in_set_char;
            if (in_offset < in_string.Length-1)
            {
                str_tmp += in_string.Substring(in_offset+1 , in_string.Length-(in_offset+1));
            }
            return str_tmp;
        }
        //ボタンクリックイベント
        private void Btn_Check_Click(object sender, RoutedEventArgs e)
        {
            this.chk_answer(this.try_no,this.Question, this.Txt_Ans.Text, this.hint_sw);
            this.Txt_Ans.Text = string.Empty;
            if (this.hit_number == this.try_no){
                if (this.level_no == 7)
                {
                    MessageBoxResult r = MessageBox.Show("全て解読完了！！\nおめでとう！", "", MessageBoxButton.OK);
                    if (r == MessageBoxResult.OK)
                    {
                        this.level_no = 1;
                        this.init(this.level_no);
                    }
                }
                else
                {
                    MessageBoxResult r = MessageBox.Show("解読完了！！\n次のレベルへチャレンジ！", "", MessageBoxButton.OK);
                    if (r == MessageBoxResult.OK)
                    {
                        this.level_no +=1;
                        this.init(this.level_no);
                    }
                }
            }
            this.Txt_Hint_MSG.Text = "ヒントの使用回数はあと" + this.hint_count + "回です";
        }
        /// <summary>
        /// トグルスイッチをオフにした際のイベントハンドラー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>_
        private void toggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
          //  Debug.WriteLine("トグルスイッチオフ！！");
            this.hint_sw = false;

         //   this.toggleSwitch.Foreground = new SolidColorBrush(Colors.Red);
        }
        /// <summary>
        /// トグルスイッチをオンにした際のイベントハンドラー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
           // Debug.WriteLine("トグルスイッチオン！！");
            this.hint_sw = true;
 
         //   this.toggleSwitch.Foreground = new SolidColorBrush(Colors.Red);
            this.toggleSwitch.SwitchForeground = new SolidColorBrush(Colors.Yellow);
        }

    }
}