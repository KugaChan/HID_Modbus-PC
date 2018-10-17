https://github.com/KugaChan/HID_Modbus-PC.git

Git11
1. 去掉定时发送FIFO的设定，只要modbus非忙碌，就把FIFO里的指令发出去，效率最大化

Git10
1. 增加identify按钮获得HID的USB连接状态
2. 增加reconnect按钮，让HID在USB连接异常时重连一次

Git9
1. 通过NumberLock的状态去决定数字键盘是控制鼠标还是输入数字
2. 默认焦点放在按键上

Git8
1. 调整定时器时间为10ms，不至于太卡
2. 修正之前鼠标移动和点击不能使用的问题
3. 可发送命令让USB设备复位

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