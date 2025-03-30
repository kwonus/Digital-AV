#include <directory.h>

namespace avx
{
    directory::directory(XVMem<byte>& xvmem) : memory(xvmem)
    {
        ;
    }
    directory::~directory()
    {
        ;
    }
    const artifact* directory::get_artifact(const char label[])
    {
        artifact* entry = (artifact*)memory.GetData();
        u32 cnt = entry[0].record_cnt;
        u32 len = entry[0].record_len;

        assert(len == sizeof(artifact));
        assert(Strnicmp(entry[0].label, DIRECTORY, sizeof(artifact::label) - 1) == 0);

        for (u32 i = 0; i < cnt; i++)
        {
            if (Strnicmp(entry[i].label, label, sizeof(artifact::label) - 1) == 0)
                return entry + i;
        }
        return nullptr;
    }
}