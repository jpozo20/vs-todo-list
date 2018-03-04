# VsToDoList

Visual Studio extension for managing tasks and To-Dos and avoid context switching when using an external program.

## Description
This is a Visual Studio extension for handling tasks and to-dos inside Visual Studio and avoid context switching when using an external program. For now, you can add, edit and remove tasks and sub-tasks, with more features like sorting,priority,etc coming with time.

The tool is available on the View -> Other Windows menu

## Screenshots

![ScreenShot-1](https://i1.visualstudiogallery.msdn.s-msft.com/3a791b9b-7dcc-4b19-bdc3-4bbd7f3c1061/image/file/275206/1/screenshot-1.png)

## Changelog
* v0.2.2
  * Fixed a bug when tasks would not load if the tool window wasn't active at the startup of VS
* v0.2.1
  * Fixed some bugs
  * Added the ability to save and load tasks on a solution basis
  * Tasks marked as Done will have a strikethrough decoration
  * 
* v0.2.0
  * When a child task is marked as done, its parent will update its status accordingly. And vice-versa, when a parent task is marked as done, all child tasks will be marked as done
  * If a task has multiple childs, and only one of them is marked done, then the parent task will be marked as semi-done or half-done
   


## License
This project is under the MIT License.
