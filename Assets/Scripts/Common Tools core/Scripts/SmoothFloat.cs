

public class SmoothFloat
	{
	float m_sum;
	float m_smoothValue;

	float[] m_buffer;
	int m_size = 0;
	int m_start;
	int m_end;
	int m_count;


	public SmoothFloat (int size)
		{
		if (size < 0) size = 0;

		m_buffer = new float[size];
		m_size = size;

		Reset();
		}


	public void SetValue (float value)
		{
		if (m_size == 0)
			{
			m_smoothValue = value;
			return;
			}

		// Enqueue the received value in the buffer

		m_buffer[m_end++] = value;
		if (m_end >= m_size) m_end = 0;
		m_count++;

		// Add the value to the accumulated sum and compute the actual smoothed value

		m_sum += value;
		m_smoothValue = m_sum / m_count;

		// If buffer is full, dequeue the older element and remove it from the sum

		if (m_count == m_size)
			{
			m_sum -= m_buffer[m_start++];
			if (m_start >= m_size) m_start = 0;
			m_count--;
			}
		}


	public float GetValue ()
		{
		return m_smoothValue;
		}


	public void Reset ()
		{
		m_sum = 0.0f;
		m_smoothValue = 0.0f;

		m_start = 0;
		m_end = 0;
		m_count = 0;
		}


	public void Resize (int targetSize)
		{
		if (targetSize < 0) targetSize = 0;

		if (targetSize != m_size)
			{
			// Buffer is reset to the actual smoothed value

			m_buffer = new float[targetSize];
			m_size = targetSize;

			float currentValue = m_smoothValue;
			Reset();

			// Store the actual value

			SetValue(currentValue);
			}
		}
	}

