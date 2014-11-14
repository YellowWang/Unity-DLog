##unity log
unity 的Debug.Log的问题

* 只有三种分类：普通、warning、error，log太多了不好看
* 不能搜索，很难找出关键想要的信息
* 跟踪堆栈信息的时候，跳转只能手动去特定文件特定行 

##DLog

DLog特性：

* 多种分类，可以简单的添加
* 支持搜索，快速筛选出想要的log
* 堆栈上的所有文件都支持双击跳转
* 截获Debug.Log，显示在DLog里
* copy按钮可以复制整个log

TODO：

* 严重性可以用slider调节
* 不同平台特定输出函数

不足：

* log区域和堆栈区域是定死的，不能用鼠标拖动
* log太长copy按钮被顶出去