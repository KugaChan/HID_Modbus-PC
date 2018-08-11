https://github.com/KugaChan/HID_Modbus-PC.git

Git7
1. 加入eKey功能，间接实现复制粘贴功能
2. 通过定时器发送FIFO，更加稳定

Git6
1. 调整一下案件布局，与触摸屏方案一致

Git5
1. 优化组合键的发送逻辑

Git4
1. 加入Ctrl、Alt和Shift的按键本身的单击发送
2. 加入数字小键盘的热键功能

Git3
1. 加入黏合Ctrl、Alt和Shift的功能，当特殊组合键与本机重复时，可以通过鼠标点击黏合来发出组合键

Git2
1. 加入重复启动软件报警
2. Ctrl, Alt和Shift可单击
3. 加入modbus响应超时提醒

Git1
1. 第一个Git版本，PC端软件

PC通过串口走Modbus协议与STM32虚拟键鼠通讯，实现界面键鼠操作其他电脑