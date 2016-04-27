## Copyright (C) 2016  Kevin Bloom <kdb5@pct.edu>
##
## This program is free software: you can redistribute it and/or modify
## it under the terms of the GNU General Public License as published by
## the Free Software Foundation, either version 3 of the License, or
## (at your option) any later version.
## 
## This program is distributed in the hope that it will be useful,
## but WITHOUT ANY WARRANTY; without even the implied warranty of
## MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
## GNU General Public License for more details.
## 
## You should have received a copy of the GNU General Public License
## along with this program.  If not, see <http://www.gnu.org/licenses/>.

## Run this function so it's easier to get data from sleep or usleep
splitSleeps <- function(allData){
  sleeps <- paste(unique(allData["Mode"])$Mode)
  return(Map(function(mode){ 
    return(subset(allData, Mode == mode))
  }, sleeps))
}
