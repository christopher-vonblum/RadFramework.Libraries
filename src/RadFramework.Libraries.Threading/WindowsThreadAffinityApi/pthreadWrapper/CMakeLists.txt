cmake_minimum_required(VERSION 3.21)
project(pthreadWrapper C)

set(CMAKE_C_STANDARD 99)

add_library(pthreadWrapper pthreadWrapper.c)
