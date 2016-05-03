# Testing the Accuracy of sleep() and usleep()

## Summary
In this project, I will be measuring the accuracy of the unistd (C) functions
sleep and usleep. The goal of the project is not just to see how accurate the
functions are but how accurate the DSO is and see how long it takes the board
to run the functions and set the output pin high/low.

The project will consist of 2 parts:
*  ZYBO
*  C#

The ZYBO will use the private timer to measure the sleep period. This value
will be shipped to C# after it is complete. C# will control the DSO to measure
the on time of the signal coming out of the ZYBO. This is what I will
demonstrate.

## Code
The ZYBO project will basically accept some value from C# via serial then pass 
that value into the sleep or usleep function. Right before the sleep (or 
usleep) function is executed, the private timer will start counting and an 
output will go high (pin JB2). This will allow both tests to be done at the 
same time.

The C# project will send the ZYBO a number, this number being the amount of 
time the user wishes to sleep. After sending out the number it will tell the DSO
to do its thing. The DSO will adjust the time and voltage scales to get a
waveform on-screen. Then it will grab all the data and ship it to C#. C# will
then ask the DSO what the time increment value is and multiple it by the 
number of data points (from the DSO) above 3V (ON).
Then both values (private timer and DSO) will be placed into a C# spreadsheet
and copied into an excel spreadsheet when done. 

## Testing
To get an ample amount of data, I will run each test 10 times with 10 different
values. These values ranging from small to large. For example, for the sleep
function I plan on going from 1 second to 30 seconds. For usleep I will probably
do something like 1 usecond to 1 second. Each second value will be done ten
times to see how consistent the functions are. 

## Data Analysis
There will mainly be percentage errors (and differences) between the 
theoretical, private, and DSO data. I will also graph the data to see if it gets
less accurate with longer times. I suppose I could do some standard deviations 
to see if the data is consistent. 
