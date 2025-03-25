# CalculatorApp
  This project aims to develop a Calculator application using C# and WPF (Windows Presentation Foundation), designed to resemble the default Windows Calculator in Standard mode while incorporating selected features from the Programmer mode. The application will support:
  
    •	Basic arithmetic operations: Addition (+), Subtraction (-), Multiplication (*), Division (/), Modulus (%), Square root, Square (x²), Reciprocal (1/x), and Negation (+/-).
    •	Memory operations: Including Memory Clear (MC), Memory Recall (MR), Memory Store (MS), Memory Add (M+), Memory Subtract (M-), and a Memory Stack display (M>).
    •	Editing functions: Backspace (deletes one digit), Clear Entry (CE - removes the last entered number), and Clear (C - resets all input).
    •	Clipboard operations: Cut, Copy, and Paste functionality, implemented without relying on pre-built clipboard features of TextBox or TextBlock.
    •	Digit grouping: Numbers should be displayed with thousands separators according to the system’s regional settings (e.g., "1,000" in UK/US, "1.000" in Romania).
    •	Programmer Mode: Supports number conversion between bases (Binary, Octal, Decimal, Hexadecimal), while calculations remain in base 10.

  Technical Requirements & Persistence
    The application remembers user settings between sessions, storing:
    
      1.	Digit grouping preference (enabled/disabled).
      2.	Last used mode (Standard or Programmer).
      3.	Last selected numerical base in Programmer mode.

  Technologies Used
  
    •	Programming Language: C#
    •	Framework: WPF (Windows Presentation Foundation)
    •	Data Persistence: .NET User Settings or encrypted file storage
    •	UI Development: Custom controls, menus, and buttons using WPF
    •	Regional Formatting: CultureInfo for localized number formatting


