using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using ButterflyControl;

namespace Butterflies
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }
    #region Variables
    Storyboard mButterfly1Board = new Storyboard();
    Storyboard mButterfly2Board = new Storyboard();
    Storyboard mButterfly3Board = new Storyboard();
    Storyboard mButterfly4Board = new Storyboard();
    Storyboard mButterfly4bBoard = new Storyboard();
    Storyboard mRectangle0Board = new Storyboard();
    Storyboard mRectangle1Board = new Storyboard();
    Storyboard mRectangle2Board = new Storyboard();
    Storyboard mMainClipBoard = new Storyboard();
    Random mRandom = new Random(DateTime.Now.TimeOfDay.Seconds);
    MediaPlayer mMediaPlayer1 = new MediaPlayer();
    MediaPlayer mMediaPlayer2 = new MediaPlayer();
    MediaPlayer mMediaPlayer3 = new MediaPlayer();
    MediaPlayer mMediaPlayer4 = new MediaPlayer();

    int mButterflyDuration = 3;         // time in seconds for butterfly video sections to move
    int mMediaElementHeight;
    int mMediaElementWidth;
    double mClipLength;
    int mXmargin;
    int mYmargin;
    double mFinalX;
    double mFinalY;
    double mA1StartX;
    double mA1StartY;
    double mA2StartX;
    double mA2StartY;
    double mA3StartX;
    double mA3StartY;
    double mA4StartX;
    double mA4StartY;
    double mButterflyX;
    double mButterflyY;
    #endregion

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Application.Current.Shutdown();
    }

    private void Video1_MediaFailed(object sender, ExceptionRoutedEventArgs e)
    {
      MessageBox.Show("Media failed");
    }

    #region Loaded Event
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        AdjustToScreenSize();

        Setup_Butterfly1_Animation();
        Setup_Butterfly2_Animation();
        Setup_Butterfly3_Animation();
        Setup_Butterfly4_Animation();

        Setup_Butterfly4b_Animation();

        Setup_Rec0FadeIn_Animation();
        Setup_Rectangle1_Animation();
        Setup_Rectangle2_Animation();

        Setup_MainClipAnimation();

        mButterfly1Board.Begin(this);
        mButterfly2Board.Begin(this);
        mButterfly3Board.Begin(this);
        mButterfly4Board.Begin(this);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Unable to play video\n\n" + ex.Message);
      }
    }

    /// <summary>
    /// Adjust to size of the grid control in the window (which is 'maximized').
    /// </summary>
    void AdjustToScreenSize()
    {
      // Calculate size for MediaElements and their 'clips'
      mMediaElementHeight = (int)MainGrid.ActualHeight / 2;
      mMediaElementWidth = mMediaElementHeight;
      mClipLength = mMediaElementWidth / 2;

      // Don't show rectangles yet
      Rec0.Opacity = 0;
      Rec1.Opacity = 0;
      Rec2.Opacity = 0;

      // Calculate margin around screen
      mXmargin = (int)MainGrid.ActualWidth / 8;
      mYmargin = (int)MainGrid.ActualHeight / 16;

      // Calculate starting points (X, Y) for all 4 butterfly animations
      mA1StartX = mXmargin;
      mA1StartY = mYmargin;
      mA2StartX = (int)MainGrid.ActualWidth - mMediaElementWidth - mXmargin;
      mA2StartY = mYmargin;
      mA3StartX = mXmargin;
      mA3StartY = (int)MainGrid.ActualHeight - mMediaElementHeight - mYmargin;
      mA4StartX = (int)MainGrid.ActualWidth - mMediaElementWidth - mXmargin;
      mA4StartY = (int)MainGrid.ActualHeight - mMediaElementHeight - mYmargin;

      Video1.Margin = new Thickness(mA1StartX, mA1StartY, 0, 0);
      Video2.Margin = new Thickness(mA2StartX, mA2StartY, 0, 0);
      Video3.Margin = new Thickness(mA3StartX, mA3StartY, 0, 0);
      Video4.Margin = new Thickness(mA4StartX, mA4StartY, 0, 0);

      // Calculate final point (X, Y) for all 4 butterfly animations
      mFinalX = ((int)MainGrid.ActualWidth - mMediaElementWidth) / 2;
      mFinalY = ((int)MainGrid.ActualHeight - mMediaElementHeight) / 2;

      Video1.Height = mMediaElementHeight;
      Video1.Width = mMediaElementWidth;
      Video2.Height = mMediaElementHeight;
      Video2.Width = mMediaElementWidth;
      Video3.Height = mMediaElementHeight;
      Video3.Width = mMediaElementWidth;
      Video4.Height = mMediaElementHeight;
      Video4.Width = mMediaElementWidth;

      // Set size of 'clips' for each MediaElement
      VideoClip1.Rect = new Rect(0, 0, mClipLength, mClipLength);
      VideoClip2.Rect = new Rect(mClipLength, 0, mClipLength, mClipLength);
      VideoClip3.Rect = new Rect(0, mClipLength, mClipLength, mClipLength);
      VideoClip4.Rect = new Rect(mClipLength, mClipLength, mClipLength, mClipLength);

      // Set location and size of rectangles now, but we'll animate them later
      Rec0.Margin = new Thickness(mFinalX, mFinalY, 0, 0);
      Rec1.Margin = new Thickness(mFinalX, mFinalY, 0, 0);
      Rec2.Margin = new Thickness(mFinalX, mFinalY, 0, 0);
      Rec0.Height = mMediaElementHeight;
      Rec0.Width = mMediaElementWidth;
      Rec1.Height = mMediaElementHeight;
      Rec1.Width = mMediaElementWidth;
      Rec2.Height = mMediaElementHeight;
      Rec2.Width = mMediaElementWidth;

      // Setup the clip area which is used to close the entire window at the end of all animations.
      double ClipX = MainGrid.ActualWidth / 2;
      double ClipY = MainGrid.ActualHeight / 2;
      MainClip.Center = new Point(ClipX, ClipY);
      MainClip.RadiusX = MainGrid.ActualWidth;
      MainClip.RadiusY = MainGrid.ActualHeight;

      // Place level 1 butterfly in the centre
      mButterflyX = (MainGrid.ActualWidth - Butterfly1.ActualWidth) / 2;
      mButterflyY = (MainGrid.ActualHeight - Butterfly1.ActualHeight) / 2;
      Butterfly1.Margin = new Thickness(mButterflyX, mButterflyY, 0, 0);

      // Place level 2 butterflies in a circle near the centre
      Butterfly2a.Margin = new Thickness(mButterflyX, mButterflyY - 100, 0, 0);        // angle = 0
      Butterfly2b.Margin = new Thickness(mButterflyX + 25, mButterflyY - 75, 0, 0);
      Butterfly2c.Margin = new Thickness(mButterflyX +50, mButterflyY -50, 0, 0);
      Butterfly2d.Margin = new Thickness(mButterflyX +75, mButterflyY -25, 0, 0);
      Butterfly2e.Margin = new Thickness(mButterflyX +100, mButterflyY, 0, 0);        // angle = 90
      Butterfly2f.Margin = new Thickness(mButterflyX + 75, mButterflyY + 25, 0, 0);
      Butterfly2g.Margin = new Thickness(mButterflyX + 50, mButterflyY + 50, 0, 0);
      Butterfly2h.Margin = new Thickness(mButterflyX + 25, mButterflyY + 75, 0, 0);
      Butterfly2i.Margin = new Thickness(mButterflyX + 0, mButterflyY + 100, 0, 0);   // angle = 180
      Butterfly2j.Margin = new Thickness(mButterflyX - 25, mButterflyY + 75, 0, 0);
      Butterfly2k.Margin = new Thickness(mButterflyX - 50, mButterflyY + 50, 0, 0);
      Butterfly2l.Margin = new Thickness(mButterflyX - 75, mButterflyY + 25, 0, 0);
      Butterfly2m.Margin = new Thickness(mButterflyX - 100, mButterflyY + 0, 0, 0);     // angle = 270
      Butterfly2n.Margin = new Thickness(mButterflyX - 75, mButterflyY - 25, 0, 0);
      Butterfly2o.Margin = new Thickness(mButterflyX - 50, mButterflyY - 50, 0, 0);
      Butterfly2p.Margin = new Thickness(mButterflyX - 25, mButterflyY - 75, 0, 0);

      // Invisible Butterfly1's duration controls when the window clip closes the window and stops this program.
      Butterfly1.AnimationDuration = 12.0;
      Butterfly1.MoveAnimationCompleted += Butterfly1_MoveAnimationCompleted;
      Butterfly1.AnimationDelay = 0.0;

      // Set delay times so butterflies don't all start at the same time.
      double delayInternal = 0.5;
      Butterfly2a.AnimationDelay = 0.0;
      Butterfly2b.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2c.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2d.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2e.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2f.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2g.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2h.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2i.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2j.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2k.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2l.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2m.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2n.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2o.AnimationDelay = delayInternal * mRandom.Next(1, 10);
      Butterfly2p.AnimationDelay = delayInternal * mRandom.Next(1, 10);
    }

    void Setup_Butterfly1_Animation()
    {
      ThicknessAnimationUsingKeyFrames animation = new ThicknessAnimationUsingKeyFrames();

      animation.Duration = TimeSpan.FromSeconds(mButterflyDuration);
      animation.BeginTime = TimeSpan.FromSeconds(2);
      animation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(mA1StartX, mA1StartY, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
      animation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(mFinalX, mFinalY, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(mButterflyDuration))));

      Storyboard.SetTargetName(animation, "Video1");
      Storyboard.SetTargetProperty(animation, new PropertyPath(MediaElement.MarginProperty));
      mButterfly1Board.Children.Add(animation);
    }

    void Setup_Butterfly2_Animation()
    {
      ThicknessAnimationUsingKeyFrames animation = new ThicknessAnimationUsingKeyFrames();

      animation.Duration = TimeSpan.FromSeconds(mButterflyDuration);
      animation.BeginTime = TimeSpan.FromSeconds(2);
      animation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(mA2StartX, mA2StartY, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
      animation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(mFinalX, mFinalY, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(mButterflyDuration))));

      Storyboard.SetTargetName(animation, "Video2");
      Storyboard.SetTargetProperty(animation, new PropertyPath(MediaElement.MarginProperty));
      mButterfly2Board.Children.Add(animation);
    }

    void Setup_Butterfly3_Animation()
    {
      ThicknessAnimationUsingKeyFrames animation = new ThicknessAnimationUsingKeyFrames();

      animation.Duration = TimeSpan.FromSeconds(mButterflyDuration);
      animation.BeginTime = TimeSpan.FromSeconds(2);
      animation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(mA3StartX, mA3StartY, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
      animation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(mFinalX, mFinalY, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(mButterflyDuration))));

      Storyboard.SetTargetName(animation, "Video3");
      Storyboard.SetTargetProperty(animation, new PropertyPath(MediaElement.MarginProperty));
      mButterfly3Board.Children.Add(animation);
    }

    void Setup_Butterfly4_Animation()
    {
      ThicknessAnimationUsingKeyFrames animation = new ThicknessAnimationUsingKeyFrames();
      animation.Completed += Butterfly4Animation_Completed;

      animation.Duration = TimeSpan.FromSeconds(mButterflyDuration);
      animation.BeginTime = TimeSpan.FromSeconds(2);
      animation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(mA4StartX, mA4StartY, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
      animation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(mFinalX, mFinalY, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(mButterflyDuration))));

      Storyboard.SetTargetName(animation, "Video4");
      Storyboard.SetTargetProperty(animation, new PropertyPath(MediaElement.MarginProperty));
      mButterfly4Board.Children.Add(animation);
    }

    /// <summary>
    /// To fade out butterfly video
    /// </summary>
    void Setup_Butterfly4b_Animation()
    {
      DoubleAnimation animation = new DoubleAnimation();
      animation.Duration = TimeSpan.FromSeconds(9);
      animation.From = 1;
      animation.To = 0;
      animation.AutoReverse = false;
      animation.FillBehavior = FillBehavior.HoldEnd;

      Storyboard.SetTargetName(animation, "Video4");
      Storyboard.SetTargetProperty(animation, new PropertyPath(MediaElement.OpacityProperty));
      mButterfly4bBoard.Children.Add(animation);
    }

    /// <summary>
    /// To fade in/out rectangle 0
    /// </summary>
    void Setup_Rec0FadeIn_Animation()
    {
      DoubleAnimation animation = new DoubleAnimation();
      animation.Duration = TimeSpan.FromSeconds(5);   // total 10 seconds
      animation.From = 0;
      animation.To = .7;
      animation.AutoReverse = true;
      animation.FillBehavior = FillBehavior.HoldEnd;
      animation.Completed += Rec0Fade_Completed;

      Storyboard.SetTargetName(animation, "Rec0");
      Storyboard.SetTargetProperty(animation, new PropertyPath(Rectangle.OpacityProperty));
      mRectangle0Board.Children.Add(animation);
    }

    void Setup_Rectangle1_Animation()
    {
      DoubleAnimation animation = new DoubleAnimation();

      animation.Duration = TimeSpan.FromSeconds(3);
      animation.From = 0;
      animation.To = .7;
      animation.AutoReverse = true;
      animation.FillBehavior = FillBehavior.HoldEnd;
      //animation.RepeatBehavior = RepeatBehavior.Forever;
      //      animation.BeginTime = TimeSpan.FromSeconds(2);

      Storyboard.SetTargetName(animation, "Rec1");
      Storyboard.SetTargetProperty(animation, new PropertyPath(Rectangle.OpacityProperty));
      mRectangle1Board.Children.Add(animation);
    }

    void Setup_Rectangle2_Animation()
    {
      DoubleAnimation animation = new DoubleAnimation();

      animation.BeginTime = TimeSpan.FromSeconds(2);
      animation.Duration = TimeSpan.FromSeconds(3);
      animation.From = 0;
      animation.To = .8;
      animation.AutoReverse = true;
      //      animation.RepeatBehavior = RepeatBehavior.Forever;
      animation.FillBehavior = FillBehavior.HoldEnd;

      Storyboard.SetTargetName(animation, "Rec2");
      Storyboard.SetTargetProperty(animation, new PropertyPath(Rectangle.OpacityProperty));
      mRectangle2Board.Children.Add(animation);
    }


    /// <summary>
    /// To close the window at the end.
    /// </summary>
    void Setup_MainClipAnimation()
    {
      double ClipDuration = 5;
      DoubleAnimation radiusXanimation = new DoubleAnimation();
      radiusXanimation.Duration = TimeSpan.FromSeconds(ClipDuration);
      radiusXanimation.To = 0;
      radiusXanimation.AutoReverse = false;
      radiusXanimation.FillBehavior = FillBehavior.HoldEnd;
      radiusXanimation.Completed += MainClip_Completed;

      DoubleAnimation radiusYanimation = new DoubleAnimation();
      radiusYanimation.Duration = TimeSpan.FromSeconds(ClipDuration);
      radiusYanimation.To = 0;
      radiusYanimation.AutoReverse = false;
      radiusYanimation.FillBehavior = FillBehavior.HoldEnd;

      Storyboard.SetTargetName(radiusXanimation, "MainClip");
      Storyboard.SetTargetProperty(radiusXanimation, new PropertyPath(EllipseGeometry.RadiusXProperty));
      mMainClipBoard.Children.Add(radiusXanimation);

      Storyboard.SetTargetName(radiusYanimation, "MainClip");
      Storyboard.SetTargetProperty(radiusYanimation, new PropertyPath(EllipseGeometry.RadiusYProperty));
      mMainClipBoard.Children.Add(radiusYanimation);
    }
    #endregion

    /// <summary>
    /// When the Butterfly video sections merge together, remove the clip from one of them.
    /// That way the video continues with a crisp image that avoids sync problems from multi videos.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Butterfly4Animation_Completed(object sender, EventArgs e)
    {
      Video4.Clip = null;
      Video1.Opacity = 0;
      Video2.Opacity = 0;
      Video3.Opacity = 0;

      // Setup and start bird sounds.
      SetupMediaPlayer1();
    }

    #region Play bird sounds
    /// <summary>
    /// Setup the first of a series of audio files of bird sounds.
    /// </summary>
    void SetupMediaPlayer1()
    {
      mMediaPlayer1.MediaFailed += mMediaPlayer1_MediaFailed;
      mMediaPlayer1.MediaEnded += mMediaPlayer1_MediaEnded;
      mMediaPlayer1.MediaOpened += mMediaPlayer1_MediaOpened;
      mMediaPlayer1.Volume = 0.3;
      mMediaPlayer1.Open(new Uri("Sounds/yellowwarbler1.wav", UriKind.Relative));
    }

    // Play birds audio 1
    void mMediaPlayer1_MediaOpened(object sender, EventArgs e)
    {
      mMediaPlayer1.MediaOpened -= mMediaPlayer1_MediaOpened;
      mMediaPlayer1.Play();
    }

    // Birds audio 1 finished.
    void mMediaPlayer1_MediaEnded(object sender, EventArgs e)
    {
      mMediaPlayer1.Stop();
      mMediaPlayer1.Close();
      mMediaPlayer1.MediaEnded -= mMediaPlayer1_MediaEnded;
      mMediaPlayer1.MediaFailed -= mMediaPlayer1_MediaFailed;

      mMediaPlayer2.MediaEnded += mMediaPlayer2_MediaEnded;
      mMediaPlayer2.MediaFailed += mMediaPlayer2_MediaFailed;
      mMediaPlayer2.MediaOpened += mMediaPlayer2_MediaOpened;
      mMediaPlayer2.Volume = 0.3;
      mMediaPlayer2.Open(new Uri("Sounds/luwa1.wav", UriKind.Relative));
    }

    // Play birds audio 2
    void mMediaPlayer2_MediaOpened(object sender, EventArgs e)
    {
      mMediaPlayer2.MediaOpened -= mMediaPlayer2_MediaOpened;
      mMediaPlayer2.Play();
    }

    // Birds audio 2 finished.
    void mMediaPlayer2_MediaEnded(object sender, EventArgs e)
    {
      mMediaPlayer2.Stop();
      mMediaPlayer2.Close();
      mMediaPlayer2.MediaEnded -= mMediaPlayer2_MediaEnded;
      mMediaPlayer2.MediaFailed -= mMediaPlayer2_MediaFailed;

      mMediaPlayer3.MediaEnded += mMediaPlayer3_MediaEnded;
      mMediaPlayer3.MediaFailed += mMediaPlayer3_MediaFailed;
      mMediaPlayer3.MediaOpened += mMediaPlayer3_MediaOpened;
      mMediaPlayer3.Volume = 0.3;
      mMediaPlayer3.Open(new Uri("Sounds/bswallow3.wav", UriKind.Relative));
      //      mMediaPlayer3.Open(new Uri("Sounds/luwa1.wav", UriKind.Relative));
    }

    // Play birds audio 3
    void mMediaPlayer3_MediaOpened(object sender, EventArgs e)
    {
      mMediaPlayer3.MediaOpened -= mMediaPlayer3_MediaOpened;
      mMediaPlayer3.Play();
    }

    // Birds audio 3 finished.
    void mMediaPlayer3_MediaEnded(object sender, EventArgs e)
    {
      mMediaPlayer3.Stop();
      mMediaPlayer3.Close();
      mMediaPlayer3.MediaEnded -= mMediaPlayer3_MediaEnded;
      mMediaPlayer3.MediaFailed -= mMediaPlayer3_MediaFailed;

      mMediaPlayer4.MediaEnded += mMediaPlayer4_MediaEnded;
      mMediaPlayer4.MediaFailed += mMediaPlayer4_MediaFailed;
      mMediaPlayer4.MediaOpened += mMediaPlayer4_MediaOpened;
      mMediaPlayer4.Volume = 0.3;
      mMediaPlayer4.Open(new Uri("Sounds/tuti2.wav", UriKind.Relative));
    }

    // Play birds audio 4
    void mMediaPlayer4_MediaOpened(object sender, EventArgs e)
    {
      mMediaPlayer4.MediaOpened -= mMediaPlayer4_MediaOpened;
      mMediaPlayer4.Play();
    }

    // Birds audio 4 finished.
    void mMediaPlayer4_MediaEnded(object sender, EventArgs e)
    {
      mMediaPlayer4.Stop();
      mMediaPlayer4.Close();
      mMediaPlayer4.MediaEnded -= mMediaPlayer4_MediaEnded;
      mMediaPlayer4.MediaFailed -= mMediaPlayer4_MediaFailed;
    }

    void mMediaPlayer1_MediaFailed(object sender, ExceptionEventArgs e)
    {
//      MessageBox.Show("Audio error - MediaPlayer1 failed");
    }

    void mMediaPlayer2_MediaFailed(object sender, ExceptionEventArgs e)
    {
//      MessageBox.Show("Audio error - MediaPlayer2 failed");
    }

    void mMediaPlayer3_MediaFailed(object sender, ExceptionEventArgs e)
    {
//      MessageBox.Show("Audio error - MediaPlayer3 failed");
    }

    void mMediaPlayer4_MediaFailed(object sender, ExceptionEventArgs e)
    {
//      MessageBox.Show("Audio error - MediaPlayer4 failed");
    }
    #endregion

    private void Video4_MediaEnded(object sender, RoutedEventArgs e)
    {
      // Fade out butterfly video
      mButterfly4bBoard.Begin(this);

      // Fade in Rectangles over the ended video area.
      mRectangle0Board.Begin(this);
      mRectangle1Board.Begin(this);
      mRectangle2Board.Begin(this);

      // Fade in animatable butterfly controls according to the FadeInDurationProperty
//      Butterfly1.FadeInAnimationActive = true;    // keep this one not visible
      Butterfly2a.FadeInAnimationActive = true;
      Butterfly2b.FadeInAnimationActive = true;
      Butterfly2c.FadeInAnimationActive = true;
      Butterfly2d.FadeInAnimationActive = true;
      Butterfly2e.FadeInAnimationActive = true;
      Butterfly2f.FadeInAnimationActive = true;
      Butterfly2g.FadeInAnimationActive = true;
      Butterfly2h.FadeInAnimationActive = true;
      Butterfly2i.FadeInAnimationActive = true;
      Butterfly2j.FadeInAnimationActive = true;
      Butterfly2k.FadeInAnimationActive = true;
      Butterfly2l.FadeInAnimationActive = true;
      Butterfly2m.FadeInAnimationActive = true;
      Butterfly2n.FadeInAnimationActive = true;
      Butterfly2o.FadeInAnimationActive = true;
      Butterfly2p.FadeInAnimationActive = true;
    }

    /// <summary>
    /// The Rectangle 0 finished fading in/out
    /// </summary>
    void Rec0Fade_Completed(object sender, EventArgs e)
    {
      // Start butterfly movement animations.
      Butterfly1.MoveAnimationActive = true;
      Butterfly2a.MoveAnimationActive = true;
      Butterfly2b.MoveAnimationActive = true;
      Butterfly2c.MoveAnimationActive = true;
      Butterfly2d.MoveAnimationActive = true;
      Butterfly2e.MoveAnimationActive = true;
      Butterfly2f.MoveAnimationActive = true;
      Butterfly2g.MoveAnimationActive = true;
      Butterfly2h.MoveAnimationActive = true;
      Butterfly2i.MoveAnimationActive = true;
      Butterfly2j.MoveAnimationActive = true;
      Butterfly2k.MoveAnimationActive = true;
      Butterfly2l.MoveAnimationActive = true;
      Butterfly2m.MoveAnimationActive = true;
      Butterfly2n.MoveAnimationActive = true;
      Butterfly2o.MoveAnimationActive = true;
      Butterfly2p.MoveAnimationActive = true;
    }

    /// <summary>
    /// Butterfly1 (shortest animation duration) has finished, so start closing the window.
    /// </summary>
    void Butterfly1_MoveAnimationCompleted(object sender, RoutedEventArgs e)
    {
      Butterfly1.MoveAnimationCompleted -= Butterfly1_MoveAnimationCompleted;

      mMainClipBoard.Begin(this);               // Start closing the window
    }

    /// <summary>
    /// The main clip animation finished, so stop the application - all done.
    /// </summary>
    void MainClip_Completed(object sender, EventArgs e)
    {
      Application.Current.Shutdown();
    }



  }
}
