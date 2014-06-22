#ifndef MMIOCHAINER_H
#define MMIOCHAINER_H

#include <windows.h>

namespace IronSpigot
{
struct MmioDataStructure
{
	bool m_finished;
	bool m_abort;
	HRESULT m_hrFinished;
	unsigned char m_soFar;
	WCHAR m_szEventName[MAX_PATH];
};

class MmioChainerBase
{
	HANDLE m_section;
	HANDLE m_event;
	MmioDataStructure* m_pData;

protected:
	virtual ~MmioChainerBase()
	{
		if (m_pData)
			::UnmapViewOfFile(m_pData);
	}
	MmioChainerBase(HANDLE section, HANDLE hevent)
		: m_section(section)
		, m_event(hevent)
		, m_pData(MapView(section))
	{}

public:
	HANDLE GetEventHandle() const { return m_event; }
	HANDLE GetMmioHandle()  const { return m_section; }
	MmioDataStructure* GetData() { return m_pData; }

	void Init(LPCWSTR eventName)
	{
		m_pData->m_finished = false;
		m_pData->m_soFar = 0;
		m_pData->m_hrFinished = E_PENDING;
		m_pData->m_abort = false;
		wcscpy_s(m_pData->m_szEventName, MAX_PATH, eventName);
	}
	void Abort()
	{
		m_pData->m_abort = true;
	}

	bool IsDone() const { return m_pData->m_finished; }
	unsigned char GetProgress() const { return m_pData->m_soFar; }
	HRESULT GetResult() const { return m_pData->m_hrFinished; }
	bool IsAborted() const { return m_pData->m_abort; }

	void Finished(HRESULT hr)
	{
		m_pData->m_hrFinished = hr;
		m_pData->m_finished = true;
		::SetEvent(m_event);
	}
	void SoFar(unsigned char soFar)
	{
		m_pData->m_soFar = soFar;
		::SetEvent(m_event);
	}

protected:
	static MmioDataStructure* MapView(HANDLE section)
	{
		return reinterpret_cast<MmioDataStructure*>(::MapViewOfFile(section,
																	FILE_MAP_WRITE,
																	0, 0,
																	sizeof(MmioDataStructure)));
	}
};

class MmioChainer : protected MmioChainerBase
{
public:
	MmioChainer (LPCWSTR sectionName, LPCWSTR eventName)
		: MmioChainerBase(CreateSection(sectionName), CreateEvent(eventName))
	{
		Init(eventName);
	}
	virtual ~MmioChainer ()
	{
		::CloseHandle(GetEventHandle());
		::CloseHandle(GetMmioHandle());
	}

public:
	using MmioChainerBase::Abort;
	void Run(HANDLE process)
	{
		HANDLE handles[2] = { process, GetEventHandle() };

		while(!IsDone())
		{
			DWORD ret = ::WaitForMultipleObjects(2, handles, FALSE, 100);
			switch(ret)
			{
			case WAIT_OBJECT_0:
			{
				if (IsDone() == false)
				{
					HRESULT hr = GetResult();
					if (hr == E_PENDING)
						OnFinished(E_FAIL);
					else
						OnFinished(hr);
					return;
				}
				break;
			}
			case WAIT_OBJECT_0 + 1:
				OnProgress(GetProgress());
				break;
			default:
				break;
			}		
		}
		OnFinished(GetResult());
	}

private:
	virtual void OnProgress(unsigned char soFar) = 0;
	virtual void OnFinished(HRESULT hr) = 0;

private:
	static HANDLE CreateSection(LPCWSTR sectionName)
	{
		return ::CreateFileMapping (INVALID_HANDLE_VALUE,
									NULL,
									PAGE_READWRITE,
									0,
									sizeof(MmioDataStructure),
									sectionName);
	}
	static HANDLE CreateEvent(LPCWSTR eventName)
	{
		return ::CreateEvent(NULL, FALSE, FALSE, eventName);
	}
};

class MmioChainee : protected MmioChainerBase
{
public:
	MmioChainee(LPCWSTR sectionName)
		: MmioChainerBase(OpenSection(sectionName), OpenEvent(GetEventName(sectionName)))
	{}
	virtual ~MmioChainee() {}

private:
	static HANDLE OpenSection(LPCWSTR sectionName)
	{
		return ::OpenFileMapping(FILE_MAP_WRITE,
								 FALSE,
								 sectionName);
	}
	static HANDLE OpenEvent(LPCWSTR eventName)
	{
		return ::OpenEvent (EVENT_MODIFY_STATE | SYNCHRONIZE,
							FALSE,
							eventName);
	}
	static CString GetEventName(LPCWSTR sectionName)
	{
		CString cs;

		HANDLE handle = OpenSection(sectionName);
		const MmioDataStructure* pData = MapView(handle);
		if (pData)
		{
			cs = pData->m_szEventName;
			::UnmapViewOfFile(pData);
		}
		::CloseHandle(handle);

		return cs;
	}
};
}
#endif