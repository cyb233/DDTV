﻿using DDTV_Core.SystemAssembly.ConfigModule;
using DDTV_GUI.DanMuCanvas.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DDTV_GUI.DanMuCanvas.BarrageParameters
{
    public class BarrageConfig
    {
        #region 相关数据
        /// <summary>
        /// 每次循环时字幕的显示（减小lengthList）的速度
        /// </summary>
        public decimal reduceSpeed;
        /// <summary>
        /// 单个弹幕的结束时间
        /// </summary>
        public double initFinishTime;
        /// <summary>
        /// 弹幕当前高度
        /// </summary>
        public int height = 0;
        #endregion

        #region 运行时
        private Canvas canvas;
        #endregion

        #region 初始化
        public BarrageConfig(Canvas canvas)
        {
            //InitializeColors();
            this.canvas = canvas;
            reduceSpeed = decimal.Parse("0.5");
            initFinishTime = double.Parse("10");
        }

        #endregion

        #region 运行时

        public void Barrage_Stroke(MessageInformation contentlist, int Index, bool IsSubtitle = false)
        {
            height = Index * GUIConfig.DanMuFontSize;
            Grid grid = new Grid();
            grid.Resources.Add("Stroke", new SolidColorBrush(Colors.Black));
            for (int i = 0; i < 4; i++)
            {
                TextBlock strokeTextBlock = new TextBlock();
                if (File.Exists("./typeface.ttf"))
                {
                    strokeTextBlock.FontFamily = new FontFamily(new Uri("file:///" + System.IO.Path.GetFullPath("./")), "./#typeface");
                }
                strokeTextBlock.Margin = new Thickness(i == 0 ? -2 : 0, i == 1 ? -2 : 0, i == 2 ? -2 : 0, i == 3 ? -2 : 0);
                strokeTextBlock.Text = !string.IsNullOrEmpty(contentlist.nickName) ? $"{contentlist.nickName}:{contentlist.content}" : contentlist.content;
                strokeTextBlock.FontSize = GUIConfig.DanMuFontSize;
                strokeTextBlock.FontWeight = System.Windows.FontWeights.Bold;
                strokeTextBlock.Foreground = (Brush)grid.Resources["Stroke"];
                grid.Children.Add(strokeTextBlock);
            }
            TextBlock textblock = new TextBlock();
            if(File.Exists("./typeface.ttf"))
            {
                textblock.FontFamily = new FontFamily(new Uri("file:///" + System.IO.Path.GetFullPath("./")), "./#typeface");
            }
            
            if (!string.IsNullOrEmpty(contentlist.nickName))
            {
                textblock.Text = $"{contentlist.nickName}:{contentlist.content}";
            }
            else
            {
                textblock.Text = contentlist.content;
            }
            textblock.FontSize = GUIConfig.DanMuFontSize;
            textblock.FontWeight = System.Windows.FontWeights.Bold;
            if (IsSubtitle)
            {
                byte R = Convert.ToByte(GUIConfig.SubtitleColor.Split(',')[0], 16);
                byte G = Convert.ToByte(GUIConfig.SubtitleColor.Split(',')[1], 16);
                byte B = Convert.ToByte(GUIConfig.SubtitleColor.Split(',')[2], 16);
                textblock.Foreground = new SolidColorBrush(Color.FromRgb(R, G, B));
            }
            else
            {
                byte R = Convert.ToByte(GUIConfig.DanMuColor.Split(',')[0], 16);
                byte G = Convert.ToByte(GUIConfig.DanMuColor.Split(',')[1], 16);
                byte B = Convert.ToByte(GUIConfig.DanMuColor.Split(',')[2], 16);
                textblock.Foreground = new SolidColorBrush(Color.FromRgb(R, G, B));
            }

            grid.Children.Add(textblock);

            //这里设置了弹幕的高度
            Canvas.SetTop(grid, height);
            canvas.Children.Add(grid);

            //实例化动画
            DoubleAnimation animation = new DoubleAnimation();
            Timeline.SetDesiredFrameRate(animation, 60); //如果有性能问题,这里可以设置帧数
                                                         //从右往左
            animation.From = canvas.ActualWidth;
            animation.To = 0 - (GUIConfig.DanMuFontSize * textblock.Text.Length);
            animation.Duration = TimeSpan.FromSeconds(initFinishTime);
            animation.AutoReverse = false;
            animation.Completed += (object sender, EventArgs e) =>
            {
                canvas.Children.Remove(grid);
            };

            //启动动画
            grid.BeginAnimation(Canvas.LeftProperty, animation);
        }



        /// <summary>
        /// 在Window界面上显示弹幕信息,速度和位置随机产生
        /// </summary>
        /// <param name="contentlist"></param>
        public void Barrage(MessageInformation contentlist, int Index, bool IsSubtitle = false)
        {
            height = Index * GUIConfig.DanMuFontSize;
            TextBlock textblock = new TextBlock();
            List<TextBlock> Stroke = new List<TextBlock>();
            Stroke.Add(new TextBlock());
            Stroke.Add(new TextBlock());
            Stroke.Add(new TextBlock());
            Stroke.Add(new TextBlock());

            //加上昵称显示
            if (!string.IsNullOrEmpty(contentlist.nickName))
            {
                textblock.Text = $"{contentlist.nickName}:{contentlist.content}";
                foreach (var item in Stroke)
                {
                    item.Text = $"{contentlist.nickName}:{contentlist.content}";
                }
            }
            else
            {
                textblock.Text = contentlist.content;
                foreach (var item in Stroke)
                {
                    item.Text = $"{contentlist.nickName}:{contentlist.content}";
                }
            }
            textblock.FontSize = GUIConfig.DanMuFontSize;
            textblock.FontWeight = System.Windows.FontWeights.Bold;
            foreach (var item in Stroke)
            {
                item.FontSize = GUIConfig.DanMuFontSize;
                item.FontWeight = System.Windows.FontWeights.Bold;
                textblock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            if (IsSubtitle)
            {

                byte R = Convert.ToByte(GUIConfig.SubtitleColor.Split(',')[0], 16);
                byte G = Convert.ToByte(GUIConfig.SubtitleColor.Split(',')[1], 16);
                byte B = Convert.ToByte(GUIConfig.SubtitleColor.Split(',')[2], 16);
                textblock.Foreground = new SolidColorBrush(Color.FromRgb(R, G, B));
            }
            else
            {
                byte R = Convert.ToByte(GUIConfig.DanMuColor.Split(',')[0], 16);
                byte G = Convert.ToByte(GUIConfig.DanMuColor.Split(',')[1], 16);
                byte B = Convert.ToByte(GUIConfig.DanMuColor.Split(',')[2], 16);
                textblock.Foreground = new SolidColorBrush(Color.FromRgb(R, G, B));
            }

            //这里设置了弹幕的高度
            Canvas.SetTop(textblock, height);
            canvas.Children.Add(textblock);
            //实例化动画
            DoubleAnimation animation = new DoubleAnimation();
            Timeline.SetDesiredFrameRate(animation, 60);  //如果有性能问题,这里可以设置帧数
                                                          //从右往左
            animation.From = canvas.ActualWidth;
            animation.To = 0 - (GUIConfig.DanMuFontSize * textblock.Text.Length);
            animation.Duration = TimeSpan.FromSeconds(initFinishTime);
            animation.AutoReverse = false;
            animation.Completed += (object sender, EventArgs e) =>
            {
                canvas.Children.Remove(textblock);
            };
            //启动动画
            textblock.BeginAnimation(Canvas.LeftProperty, animation);
        }


        #endregion
    }
}
