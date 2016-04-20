# Testing the Accuracy of sleep() and usleep()

## Summary
In this project, I will be measuring the accuracy of the unistd functions sleep 
and usleep. The goal of the project is not just to see how accurate the 
functions are but how accurate the DSO is and how long of a delay there is 
between setting an output pin high/low.

The project will consist of 2 parts:
* ZYBO
* C#

## Code
The ZYBO project will basically accept some value from C# via serial then pass 
that value into the sleep or usleep function. Right before the sleep (or 
usleep) function is executed, the private timer will start counting and an 
output will go high (pin JB2). This will allow both tests to be done at the 
same time.

The C# project will send the ZYBO a number, this number being the amount of 
time the user wishes to sleep. After sending out the number it will tell the DSO
to do its thing. How the DSO will do "its thing," I don't know yet. Then both 
values (private timer and DSO) will be placed into a C# spreadsheet.

The way test will be conducted has yet to be decided. I will probably run 10 
tests per time amount for 10 time amounts. For example, using the sleep 
function, I would run 10 tests (1 second to 10 seconds) and each second would 
have a total of 10 points per measurement method (10 from private timer and 10 
from the DSO). Makes sense? Sure. 

## Data Analysis

