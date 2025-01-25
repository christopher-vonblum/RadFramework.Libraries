#define _GNU_SOURCE

#include <pthread.h>
#include <sched.h>

#include "pthreadWrapper.h"

int attachCurrentThreadToCore(unsigned long int id, int core) {

    pthread_t thread = id;

    cpu_set_t set;

    CPU_ZERO(&set);

    CPU_SET(core, &set);

    return pthread_setaffinity_np(thread, sizeof(cpu_set_t), &set);
}


int detachCurrentThreadFromCore(unsigned long int id, int processorCount) {

    pthread_t thread = id;

    cpu_set_t set;

    CPU_ZERO(&set);

    for (int j = 0; j < processorCount; j++)
        CPU_SET(j, &set);

    return pthread_setaffinity_np(thread, sizeof(cpu_set_t), &set);
}

unsigned long int getNativeThreadIdFromCurrentThread() {

    pthread_t thread;

    thread = pthread_self();

    return (unsigned long int)thread;
}
