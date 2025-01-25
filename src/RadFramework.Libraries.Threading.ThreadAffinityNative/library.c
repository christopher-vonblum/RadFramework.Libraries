#define _GNU_SOURCE
#include "library.h"

#include <stdio.h>
#include <err.h>
#include <pthread.h>
#include <stdlib.h>
#include <unistd.h>

unsigned long int getCurrentThreadId() {
    unsigned long int thread;

    thread = pthread_self();

    return thread;
}

threadAffinitySetResult setCurrentThreadAffinity(unsigned long int processorIndex)
{
    int s;
    cpu_set_t cpuset;
    pthread_t thread;
    threadAffinitySetResult result;

    thread = pthread_self();

    CPU_ZERO(&cpuset);
    CPU_SET(processorIndex, &cpuset);

    result.threadId = thread;

    s = pthread_setaffinity_np(thread, sizeof(cpuset), &cpuset);

    if (s != 0) {
        result.setAffinityFailed = 1;
    }
    else {
        result.setAffinityFailed = 0;
    }

    return result;
}

int useAllCpusAgain(unsigned long int threadId, unsigned long int processorCount)
{
    int s;
    cpu_set_t cpuset;
    pthread_t thread = threadId;
    threadAffinitySetResult result;

    CPU_ZERO(&cpuset);

    for(unsigned long int i = 0; i < processorCount; i++) {
        CPU_SET(i, &cpuset);
    }

    result.threadId = thread;

    s = pthread_setaffinity_np(thread, sizeof(cpuset), &cpuset);

    if (s != 0) {
        return 1;
    }

    return 0;
}