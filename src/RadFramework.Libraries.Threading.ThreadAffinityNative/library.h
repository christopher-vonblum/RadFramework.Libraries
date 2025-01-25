#ifndef RADFRAMEWORK_LIBRARIES_THREADING_THREADAFFINITYNATIVE_LIBRARY_H
#define RADFRAMEWORK_LIBRARIES_THREADING_THREADAFFINITYNATIVE_LIBRARY_H

typedef struct {
    unsigned long int threadId;
    int setAffinityFailed;
} threadAffinitySetResult;

unsigned long int getCurrentThreadId();
threadAffinitySetResult setCurrentThreadAffinity(unsigned long int processorIndex);
int useAllCpusAgain(unsigned long int threadId, unsigned long int processorCount);

#endif //RADFRAMEWORK_LIBRARIES_THREADING_THREADAFFINITYNATIVE_LIBRARY_H
