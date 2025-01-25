#ifndef PTHREADWRAPPER_PTHREADWRAPPER_H
#define PTHREADWRAPPER_PTHREADWRAPPER_H

extern int attachThreadToCore(unsigned long int id, int core);
extern int detachThreadFromCore(unsigned long int id);
extern unsigned long int getNativeThreadIdFromCurrentThread();

#endif //PTHREADWRAPPER_PTHREADWRAPPER_H
