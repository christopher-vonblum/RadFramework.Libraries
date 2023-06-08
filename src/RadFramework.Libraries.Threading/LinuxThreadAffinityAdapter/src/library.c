#define _GNU_SOURCE
#include "library.h"
#include <pthread.h>
#include <stdlib.h>

unsigned long GetCurrentThreadId(void){
    pthread_t thread = pthread_self();
    return (unsigned long)thread;
}

void AssignAffinity(unsigned long threadId, int core){
    cpu_set_t cpuset;

    CPU_ZERO(&cpuset);
    CPU_SET(core, &cpuset);

    pthread_setaffinity_np(threadId, sizeof(cpuset), &cpuset);
}

void ResetAffinityAndCleanup(unsigned long threadId){
    cpu_set_t cpuset;

    CPU_ZERO(&cpuset);

    pthread_setaffinity_np(threadId, sizeof(cpuset), &cpuset);
}
