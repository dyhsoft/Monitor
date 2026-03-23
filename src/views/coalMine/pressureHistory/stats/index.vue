<template>
  <div class="pressure-stats-container">
    <el-row :gutter="16">
      <el-col :span="6">
        <el-card><div class="stat-item"><div class="label">测点总数</div><div class="value">15</div></div></el-card>
      </el-col>
      <el-col :span="6">
        <el-card><div class="stat-item"><div class="label">报警次数</div><div class="value warning">8</div></div></el-card>
      </el-col>
      <el-col :span="6">
        <el-card><div class="stat-item"><div class="label">最大压力(MPa)</div><div class="value">18.5</div></div></el-card>
      </el-col>
      <el-col :span="6">
        <el-card><div class="stat-item"><div class="label">平均压力(MPa)</div><div class="value">10.2</div></div></el-card>
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
.pressure-stats-container { padding: 16px; }
.stat-item { text-align: center; padding: 20px; }
.stat-item .label { font-size: 14px; color: #909399; margin-bottom: 10px; }
.stat-item .value { font-size: 28px; font-weight: bold; }
.stat-item .value.warning { color: #e6a23c; }
.chart-card { min-height: 450px; }
</style>
