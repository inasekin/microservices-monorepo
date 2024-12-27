FROM node:20-alpine AS base
WORKDIR /app

COPY package.json package-lock.json ./

RUN npm ci --legacy-peer-deps

COPY . .

CMD ["npm", "run", "dev"]
