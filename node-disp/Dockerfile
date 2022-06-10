FROM node:slim

COPY . .
RUN npm install

ENV PORT=80

ENTRYPOINT [ "node", "index.js" ]