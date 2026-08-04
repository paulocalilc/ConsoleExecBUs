using Interop.EXTRA;

namespace ConsoleExecBUs.Core
{
    public class ClasseExtra
    {

        // Objetos do EXTRA
#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
        static ExtraSystem oExtra;
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
        static ExtraSessions sessoes;
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
        static ExtraSession sessao;
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
        static ExtraScreen myScreen;
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.

        public class PropObjExtra : ExtraSystem
        {
            public static ExtraSystem OExtra
            {
                get { oExtra = new ExtraSystem(); return oExtra; }
                set { oExtra = value; }
            }

            public void Quit()
            {
                OExtra.Quit();
            }

            public void ViewStatus()
            {
                OExtra.ViewStatus();
            }

            public string GetDirectory(short sessType, short dirType)
            {
                return OExtra.GetDirectory(sessType, dirType);
            }

            public string GetSystemTime(short formatType)
            {
                return OExtra.GetSystemTime(formatType);
            }

            public string Name => OExtra.Name;

            public dynamic ActiveSession => OExtra.ActiveSession;

            public dynamic Application => OExtra.Application;

            public string DefaultFilePath { get => OExtra.DefaultFilePath; set => OExtra.DefaultFilePath = value; }

            public string FullName => OExtra.FullName;

            public dynamic Parent => OExtra.Parent;

            public string Path => OExtra.Path;

            public dynamic Sessions => OExtra.Sessions;

            public int TimeoutValue { get => OExtra.TimeoutValue; set => OExtra.TimeoutValue = value; }

            public string Version => OExtra.Version;

            public dynamic Waits => OExtra.Waits;

            public dynamic ActiveWindow => OExtra.ActiveWindow;
        }

        public class PropSessoes : ExtraSessions
        {
            public static ExtraSessions Sessoes
            {
                get { return sessoes; }
                set { sessoes = value; }
            }

            public dynamic Item(object session)
            {
                return Sessoes.Item(session);
            }

            public void CloseAll()
            {
                Sessoes.CloseAll();
            }

            public dynamic JumpNext()
            {
                return Sessoes.JumpNext();
            }

            public dynamic Open(object Name)
            {
                return Sessoes.Open(Name);
            }

            public dynamic OpenEx(object Name, int VXCtrlHandle)
            {
                return Sessoes.OpenEx(Name, VXCtrlHandle);
            }

            public dynamic Application => Sessoes.Application;

            public int Count => Sessoes.Count;

            public dynamic Parent => Sessoes.Parent;

            public int VisibleOnStartup { set => Sessoes.VisibleOnStartup = value; }
        }

        public class PropSessao : ExtraSession
        {
            public static ExtraSession Sessao
            {
                get { return sessao; }
                set { sessao = value; }
            }

            public void Activate()
            {
                Sessao.Activate();
            }

            public void Close()
            {
                Sessao.Close();
            }

            public dynamic NavigateTo(object vIndex)
            {
                return Sessao.NavigateTo(vIndex);
            }

            public dynamic ReceiveFile(object PCFileName, object HostFileName, object Quiet)
            {
                return Sessao.ReceiveFile(PCFileName, HostFileName, Quiet);
            }

            public void Save()
            {
                Sessao.Save();
            }

            public void SaveAs(object Name)
            {
                Sessao.SaveAs(Name);
            }

            public dynamic SendFile(object PCFileName, object HostFileName, object Quiet)
            {
                return Sessao.SendFile(PCFileName, HostFileName, Quiet);
            }

            public string GetDirectory(short dirType)
            {
                return Sessao.GetDirectory(dirType);
            }

            public void Reset()
            {
                Sessao.Reset();
            }

            public void PrintDisplay()
            {
                Sessao.PrintDisplay();
            }

            public void EnlargeFont()
            {
                Sessao.EnlargeFont();
            }

            public void ReduceFont()
            {
                Sessao.ReduceFont();
            }

            public void ClearHistory()
            {
                Sessao.ClearHistory();
            }

            public void TransferFile()
            {
                Sessao.TransferFile();
            }

            public void ClosePrintJob()
            {
                Sessao.ClosePrintJob();
            }

            public void ClearComm()
            {
                Sessao.ClearComm();
            }

            public dynamic WaitForDisconnect(object nTimeout)
            {
                return Sessao.WaitForDisconnect(nTimeout);
            }

            public dynamic WaitForTimer(object nMilliseconds, object bContinuous)
            {
                return Sessao.WaitForTimer(nMilliseconds, bContinuous);
            }

            public void InvokeSettingsDialog(object sPageType, object sSelectPage)
            {
                Sessao.InvokeSettingsDialog(sPageType, sSelectPage);
            }

            public dynamic TransferProgress(object sOptions)
            {
                return Sessao.TransferProgress(sOptions);
            }

            public void Title(string inTitle)
            {
                Sessao.Title(inTitle);
            }

            public void CaptureSetup(object FileName, object Append, object EnableCapture, object StripEscSeq)
            {
                Sessao.CaptureSetup(FileName, Append, EnableCapture, StripEscSeq);
            }

            public void CloseEx(object Options)
            {
                Sessao.CloseEx(Options);
            }

            public void UpdateStatusBar(object varText)
            {
                Sessao.UpdateStatusBar(varText);
            }

            public int InitializeELF(object ApplID, object nWaitTime)
            {
                return Sessao.InitializeELF(ApplID, nWaitTime);
            }

            public string Name => Sessao.Name;

            public dynamic Application => Sessao.Application;

            public string ColorScheme { get => Sessao.ColorScheme; set => Sessao.ColorScheme = value; }
            public int Connected { get => Sessao.Connected; set => Sessao.Connected = value; }
            public string FileTransferScheme { get => Sessao.FileTransferScheme; set => Sessao.FileTransferScheme = value; }

            public string FullName => Sessao.FullName;

            public short Height { get => Sessao.Height; set => Sessao.Height = value; }
            public string HotSpotScheme { get => Sessao.HotSpotScheme; set => Sessao.HotSpotScheme = value; }
            public int KeyboardLocked { get => Sessao.KeyboardLocked; set => Sessao.KeyboardLocked = value; }
            public string KeyMap { get => Sessao.KeyMap; set => Sessao.KeyMap = value; }
            public int Left { get => Sessao.Left; set => Sessao.Left = value; }

            public dynamic Parent => Sessao.Parent;

            public string Path => Sessao.Path;

            public dynamic get_QuickPads(object vIndex)
            {
                return Sessao.QuickPads[vIndex];
            }

            public bool Saved => Sessao.Saved;

            public dynamic Screen => Sessao.Screen;

            public dynamic get_Toolbars(object vIndex)
            {
                return Sessao.Toolbars[vIndex];
            }

            public int Top { get => Sessao.Top; set => Sessao.Top = value; }

            public short Type => Sessao.Type;

            public int Visible { get => Sessao.Visible; set => Sessao.Visible = value; }
            public int Width { get => Sessao.Width; set => Sessao.Width = value; }
            public short WindowState { get => Sessao.WindowState; set => Sessao.WindowState = value; }
            public string EditScheme { get => Sessao.EditScheme; set => Sessao.EditScheme = value; }
            public short FileTransferHostOS { get => Sessao.FileTransferHostOS; set => Sessao.FileTransferHostOS = value; }
            public short PageRecognitionTime { get => Sessao.PageRecognitionTime; set => Sessao.PageRecognitionTime = value; }
            public int Quiet { get => Sessao.Quiet; set => Sessao.Quiet = value; }

            public string HostName => Sessao.HostName;

            public string Font { get => Sessao.Font; set => Sessao.Font = value; }
            public string Language { get => Sessao.Language; set => Sessao.Language = value; }
            public int HoldScreen { get => Sessao.HoldScreen; set => Sessao.HoldScreen = value; }
            public int AutoConnect { get => Sessao.AutoConnect; set => Sessao.AutoConnect = value; }
            public int Local { get => Sessao.Local; set => Sessao.Local = value; }

            public dynamic FileTransferOptions => Sessao.FileTransferOptions;

            public dynamic Cursor => Sessao.Cursor;

            public dynamic Connectivity => Sessao.Connectivity;

            public dynamic MenuEdit => Sessao.MenuEdit;

            public int Menu { get => Sessao.Menu; set => Sessao.Menu = value; }
            public string MenuScheme { get => Sessao.MenuScheme; set => Sessao.MenuScheme = value; }
            public int Capture { get => Sessao.Capture; set => Sessao.Capture = value; }

            public int WindowHandle => Sessao.WindowHandle;

            public int RunMacroOnConnect { get => Sessao.RunMacroOnConnect; set => Sessao.RunMacroOnConnect = value; }
            public string StartupScript { get => Sessao.StartupScript; set => Sessao.StartupScript = value; }
        }

        public class PropMyScreen : ExtraScreen
        {
            public static ExtraScreen MyScreen
            {
                get { return myScreen; }
                set { myScreen = value; }
            }

            public void Copy()
            {
                MyScreen.Copy();
            }

            public void Cut()
            {
                MyScreen.Cut();
            }

            public void Delete()
            {
                MyScreen.Delete();
            }

            public dynamic Area(short Row, short Col, short EndRow, short EndCol, object Page, object Type)
            {
                return MyScreen.Area(Row, Col, EndRow, EndCol, Page, Type);
            }

            public string GetString(object Row, object Col, object Length, object Page)
            {
                return MyScreen.GetString(Row, Col, Length, Page);
            }

            public void MoveRelative(short Rows, short Cols, object Page)
            {
                MyScreen.MoveRelative(Rows, Cols, Page);
            }

            public void MoveTo(short Row, short Col, object Page)
            {
                MyScreen.MoveTo(Row, Col, Page);
            }

            public void Paste()
            {
                MyScreen.Paste();
            }

            public void PutString(string Text, object Row, object Col, object Page)
            {
                MyScreen.PutString(Text, Row, Col, Page);
            }

            public dynamic Search(string Text, object Row, object Col, object Page)
            {
                return MyScreen.Search(Text, Row, Col, Page);
            }

            public dynamic Select(short Row, short Col, short EndRow, short EndCol, object Page, object SelectMode)
            {
                return MyScreen.Select(Row, Col, EndRow, EndCol, Page, SelectMode);
            }

            public dynamic SelectAll()
            {
                return MyScreen.SelectAll();
            }

            public void SendInput(string Text)
            {
                MyScreen.SendInput(Text);
            }

            public void SendKeys(string Text)
            {
                MyScreen.SendKeys(Text);
            }

            public dynamic WaitForCursor(short Row, object Col, object EndRow, object EndCol, object Page, object Type)
            {
                return MyScreen.WaitForCursor(Row, Col, EndRow, EndCol, Page, Type);
            }

            public dynamic WaitForCursorMove(object Row, object Col, object Page, object Idle)
            {
                return MyScreen.WaitForCursorMove(Row, Col, Page, Idle);
            }

            public dynamic WaitForStream(object Text, object Idle, object timeout, object option)
            {
                return MyScreen.WaitForStream(Text, Idle, timeout, option);
            }

            public dynamic WaitForString(string Text, object Row, object Col, object Page, object Idle, object timeout, object option, object Mode, object EndRow, object EndCol)
            {
                return MyScreen.WaitForString(Text, Row, Col, Page, Idle, timeout, option, Mode, EndRow, EndCol);
            }

            public dynamic WaitHostQuiet(object Time)
            {
                return MyScreen.WaitHostQuiet(Time);
            }

            public dynamic WaitForKeys(object Time, object Keys, object Process)
            {
                return MyScreen.WaitForKeys(Time, Keys, Process);
            }

            public void CutAppend()
            {
                MyScreen.CutAppend();
            }

            public void CopyAppend()
            {
                MyScreen.CopyAppend();
            }

            public void PasteContinue()
            {
                MyScreen.PasteContinue();
            }

            public dynamic HostOptions()
            {
                return MyScreen.HostOptions();
            }

            public void Send(string Text)
            {
                MyScreen.Send(Text);
            }

            public void ClearScreen()
            {
                MyScreen.ClearScreen();
            }

            public short FieldAttribute(short Row, short Col)
            {
                return MyScreen.FieldAttribute(Row, Col);
            }

            public string GetStringEx(int StartRow, int StartCol, int EndRow, int EndCol, ATM_AREA Area, ATM_WRAP Wrap, int Attr, int Flags)
            {
                return MyScreen.GetStringEx(StartRow, StartCol, EndRow, EndCol, Area, Wrap, Attr, Flags);
            }

            public int PutStringEx(string Text, int StartRow, int StartCol, int EndRow, int EndCol, ATM_AREA Area, ATM_WRAP Wrap, ATM_PUTSTRING Mode, bool Fill)
            {
                return MyScreen.PutStringEx(Text, StartRow, StartCol, EndRow, EndCol, Area, Wrap, Mode, Fill);
            }

            public string Name => MyScreen.Name;

            public dynamic Application => MyScreen.Application;

            public short Col { get => MyScreen.Col; set => MyScreen.Col = value; }

            public short Cols => MyScreen.Cols;

            public dynamic Parent => MyScreen.Parent;

            public short Row { get => MyScreen.Row; set => MyScreen.Row = value; }

            public short Rows => MyScreen.Rows;

            public int Updated => MyScreen.Updated;

            public dynamic Selection => MyScreen.Selection;

            public dynamic OIA => MyScreen.OIA;

            public short InHiddenField => MyScreen.InHiddenField;

            public int Synchronous { get => MyScreen.Synchronous; set => MyScreen.Synchronous = value; }

            public short Page => MyScreen.Page;

            public dynamic LastMouseEvent => MyScreen.LastMouseEvent;
        }

    }
}