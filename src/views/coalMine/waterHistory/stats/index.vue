<template>
  <div class="water-stats-container">
    <el-row :gutter="16">
      <el-col :span="6">
        <el-card><div class="stat-item"><div class="label">水位测点</div><div class="value">8</div></div></el-card>
      </el-col>
      <el-col :span="6">
        <el-card><div class="stat-item"><div class="label">流量测点</div><div class="value">5</div></div></el-card>
      </el-col>
      <el-col :span="6">
        <el-card><div class="stat-item"><div class="label">最大排水量(m³/h)</div><div class="value">150.0</div></div></el-card>
      </el-col>
      <el-col :span="6">
        <el-card><div class="stat-item"><div class="label">报警次数</div><div class="value warning">12</div></div></el-card>
      </el-col>
    </el-row>
    <el-card class="chart-card" style="margin-top:16px">
      <div ref="chartRef" style="width:100%;height:400px"></div>
    </el-card>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue'
const chartRef = ref()
</script>
<style scoped>
.water-stats-container { padding: 16px; }
.stat-item { text-align: center; padding: 20px; }
.stat-item .label { font-size: 14px; color: #909399; margin-bottom: 10px; }
.stat-item .value { font-size: 28px; font-weight: bold; }
.stat-item .value.warning { color: #e6a23c; }
.chart-card { min-height: 450px; }
</style>
