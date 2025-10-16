const baseConfig = require('./index');

module.exports = {
  ...baseConfig,
  extends: [...baseConfig.extends, 'next/core-web-vitals'],
  env: {
    ...(baseConfig.env || {}),
    browser: true,
    es2021: true
  }
};
