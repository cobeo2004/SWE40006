# Stage 1: Build
FROM python:3.12-slim as base
WORKDIR /app

COPY requirements.txt /app/requirements.txt

RUN pip install -r requirements.txt

# Stage 2: Run
FROM base as dev

COPY . /app

EXPOSE 8000

CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000", "--reload"]
