#ifndef LINUXTHREADAFFINITYADAPTER_LIBRARY_H
#define LINUXTHREADAFFINITYADAPTER_LIBRARY_H

unsigned long GetCurrentThreadId(void);
void AssignAffinity(unsigned long threadId, int affinityMask);
void ResetAffinityAndCleanup(unsigned long threadId);

#endif //LINUXTHREADAFFINITYADAPTER_LIBRARY_H
